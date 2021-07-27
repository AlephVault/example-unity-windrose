using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.Serialization;
using MLAPI.Serialization.Pooled;
using MLAPI.Transports;

namespace AlephVault.Unity.MMO
{
    namespace Authoring
    {
        namespace Behaviours
        {
            namespace Authentication
            {
                using Support.Utils;
                using System.Threading;
                using Types;

                [RequireComponent(typeof(NetworkManager))]
                public abstract class Authenticator : MonoBehaviour
                {
                    private const string LoginMessagePrefix = "__AV:MMO__:LOGIN:DO";
                    private const string LoginOK = "__AV:MMO__:LOGIN:OK";
                    private const string LoginFailed = "__AV:MMO__:LOGIN:FAILED";
                    private const string AlreadyLoggedIn = "__AV:MMO__:LOGIN:ALREADY";
                    private const string LoginTimeout = "__AV:MMO__:LOGIN:TIMEOUT";
                    // Constants for forced login in particular.
                    private const string Forced = "FORCE-LOGIN:OK";
                    private const string UnknownForcedRealm = "FORCE-LOGIN:UNKNOWN-REALM";
                    private const string UnknownForcedAccountId = "FORCE-LOGIN:UNKNOWN-ID";

                    private NetworkManager manager;

                    /// <summary>
                    ///   The tolerance time that a connection will be kept
                    ///   without attempting a login, before a timeout message
                    ///   is sent to it. Expressed in seconds.
                    /// </summary>
                    [SerializeField]
                    private uint pendingLoginTimeout = 5;

                    // The current second fraction to be tracked in the Update
                    // method, to execute the action second-wise.
                    private float currentSecondFraction = 0;

                    // These callbacks are used when forced login is invoked.
                    private Dictionary<string, Func<object, bool>> forceLoginCallbacks = new Dictionary<string, Func<object, bool>>();

                    // The remote clients that did not yet perform a login attempt.
                    private Dictionary<ulong, uint> pendingLoginClients = new Dictionary<ulong, uint>();

                    // A mutex to interact with the pendingLoginClients list.
                    private Mutex mutex = new Mutex();

                    private void Awake()
                    {
                        manager = GetComponent<NetworkManager>();
                        pendingLoginTimeout = Values.Max(1u, pendingLoginTimeout);
                    }

                    // Registers the client-side login messages managers for
                    // OK, Failure, Timeout, and "already logged in". It also
                    // registers custom login messages. It also registers the
                    // callbacks for (remote) client connected and disconnected,
                    // to track clients that are login-pending.
                    private void Start()
                    {
                        manager.OnClientConnectedCallback += OnClientConnectedCallback;
                        manager.OnClientDisconnectCallback += OnClientDisconnectCallback;
                        CustomMessagingManager.RegisterNamedMessageHandler(AlreadyLoggedIn, (senderId, stream) =>
                        {
                            Debug.Log("Received message: AlreadyLoggedIn");
                            if (manager.IsClient)
                            {
                                OnAuthenticationAlreadyDone();
                            }
                        });
                        CustomMessagingManager.RegisterNamedMessageHandler(LoginTimeout, (senderId, stream) =>
                        {
                            Debug.Log("Received message: LoginTimeout");
                            if (manager.IsClient)
                            {
                                OnAuthenticationTimeout();
                            }
                        });
                        CustomMessagingManager.RegisterNamedMessageHandler(LoginOK, (senderId, stream) =>
                        {
                            Debug.Log("Received message: LoginOK");
                            if (manager.IsClient)
                            {
                                using (var reader = PooledNetworkReader.Get(stream))
                                {
                                    Response response = new Response();
                                    response.NetworkSerialize(reader.Serializer);
                                    OnAuthenticationOK(response);
                                }
                            }
                        });
                        CustomMessagingManager.RegisterNamedMessageHandler(LoginFailed, (senderId, stream) =>
                        {
                            Debug.Log("Received message: LoginFailed");
                            if (manager.IsClient)
                            {
                                using (var reader = PooledNetworkReader.Get(stream))
                                {
                                    Response response = new Response();
                                    response.NetworkSerialize(reader.Serializer);
                                    OnAuthenticationFailure(response);
                                }
                            }
                        });
                        RegisterLoginMethods();
                    }

                    // On update, it runs the interval and checks the
                    // clients that are timed-out on pending login,
                    // and kicks them.
                    private void Update()
                    {
                        if (manager.IsServer)
                        {
                            currentSecondFraction += Time.unscaledDeltaTime;
                            if (currentSecondFraction >= 1)
                            {
                                currentSecondFraction -= 1;
                                List<ulong> keysToIncrement = new List<ulong>();
                                foreach (ulong key in pendingLoginClients.Keys)
                                {
                                    if (pendingLoginClients[key] >= pendingLoginTimeout)
                                    {
                                        using (var buffer = PooledNetworkBuffer.Get())
                                        {
                                            Debug.LogFormat("Disconnecting client {0} due to login timeout", key);
                                            CustomMessagingManager.SendNamedMessage(LoginTimeout, key, buffer, NetworkChannel.Internal);
                                            manager.DisconnectClient(key);
                                        }
                                    }
                                    else
                                    {
                                        keysToIncrement.Add(key);
                                    }
                                }
                                foreach (ulong key in keysToIncrement)
                                {
                                    pendingLoginClients[key] += 1;
                                }
                            }
                        }
                    }

                    // Adds the remote client to the login-pending list.
                    private void OnClientConnectedCallback(ulong remoteClientId)
                    {
                        if (manager.IsServer)
                        {
                            pendingLoginClients.Add(remoteClientId, 0);
                            ClientSetup(remoteClientId);
                        }
                    }

                    // Removes the remote client from the login-pending list.
                    private void OnClientDisconnectCallback(ulong remoteClientId)
                    {
                        if (manager.IsServer)
                        {
                            ClientTeardown(remoteClientId);
                            pendingLoginClients.Remove(remoteClientId);
                        }
                    }

                    /// <summary>
                    ///   This callback sets the given client up after it has
                    ///   just connected (at this point, it is not logged in).
                    /// </summary>
                    /// <param name="clientId">The id of the client connection to setup</param>
                    protected abstract void ClientSetup(ulong clientId);

                    /// <summary>
                    ///   This callback tears the given client down after it
                    ///   has just connected (at this point, it may be logged
                    ///   in, but alternatively not logged in if this disconnection
                    ///   occurs because of timeout or too early).
                    /// </summary>
                    /// <param name="clientId">The id of the client connection to tear down</param>
                    protected abstract void ClientTeardown(ulong clientId);

                    /// <summary>
                    ///   <para>
                    ///     Registers a new server-side handler to perform a login
                    ///     using a particular implementation that expects particular
                    ///     data in the network reader and also returns a particular
                    ///     type of account id (or null if the account does not
                    ///     exist - an account id should always be returned if it
                    ///     exists, so to track it in further logging), and a string
                    ///     naming the "realm" the account belongs to (such name
                    ///     should NOT be empty if the account id exists).
                    ///   </para>
                    ///   <para>
                    ///     Handlers will be wrapped so that the execution will
                    ///     be a no-op if the network manager is not running in
                    ///     server (dedicated or host) mode.
                    ///   </para>
                    /// </summary>
                    /// <param name="messageName">The name of the message to register</param>
                    /// <param name="authenticationMethod">The method or callback to use as authenticator</param>
                    protected void RegisterLoginMethod(string messageName, Func<NetworkReader, Task<Tuple<Response, object, string>>> authenticationMethod)
                    {
                        CustomMessagingManager.RegisterNamedMessageHandler(string.Format("{0}:{1}", LoginMessagePrefix, messageName), (senderId, stream) =>
                        {
                            if (manager.IsServer)
                            {
                                if (IsLoggedIn(senderId))
                                {
                                    using (var buffer = PooledNetworkBuffer.Get())
                                    {
                                        CustomMessagingManager.SendNamedMessage(AlreadyLoggedIn, senderId, buffer, NetworkChannel.Internal);
                                    }
                                }
                                else
                                {
                                    Task<Tuple<Response, object, string>> task = null;
                                    using (var reader = PooledNetworkReader.Get(stream))
                                    {
                                        task = authenticationMethod(reader);
                                    }

                                    task.GetAwaiter().OnCompleted(() =>
                                    {
                                        Tuple<Response, object, string> result = task.Result;
                                        Debug.LogFormat("Result of login attempt: {0}", result.Item1);
                                        if (result.Item1.Success)
                                        {
                                            pendingLoginClients.Remove(senderId);
                                            OnAccountLoginOK(senderId, result.Item1, result.Item2, result.Item3, null, "");
                                            using (var buffer = PooledNetworkBuffer.Get())
                                            using (var writer = PooledNetworkWriter.Get(buffer))
                                            {
                                                Response response = result.Item1;
                                                response.NetworkSerialize(writer.Serializer);
                                                CustomMessagingManager.SendNamedMessage(LoginOK, senderId, buffer, NetworkChannel.Internal);
                                            }
                                        }
                                        else
                                        {
                                            OnAccountLoginFailed(senderId, result.Item1, result.Item2, result.Item3);
                                            using (var buffer = PooledNetworkBuffer.Get())
                                            using (var writer = PooledNetworkWriter.Get(buffer))
                                            {
                                                Response response = result.Item1;
                                                response.NetworkSerialize(writer.Serializer);
                                                CustomMessagingManager.SendNamedMessage(LoginFailed, senderId, buffer, NetworkChannel.Internal);
                                            }
                                            // For remote clients, disconnect them instantly.
                                            // TODO: Not that instantly! Instead, wait a while.
                                            // TODO: Bug source (still on 0.1.1): https://github.com/Unity-Technologies/com.unity.multiplayer.mlapi/issues/796
                                            if (!manager.IsClient || senderId != manager.LocalClientId)
                                            {
                                                manager.DisconnectClient(senderId);
                                            }
                                        }
                                    });
                                }
                            }
                        });
                    }

                    /// <summary>
                    ///   Registers a new forced login callback for a specific
                    ///   realm. Forcing a login involves using a valid account
                    ///   id and treating it as successfully logging in (so,
                    ///   any side-effect relevant to this forced login must be
                    ///   made to the account, if relevant). The callback takes
                    ///   an object with the account id and returns a boolean
                    ///   value which will be false only if the account does
                    ///   not exist for that id.
                    /// </summary>
                    /// <param name="realm">The name of the real for this forced login</param>
                    /// <param name="callback">The callback with the implementation of the forced login</param>
                    protected void RegisterForcedLoginMethod(string realm, Func<object, bool> callback)
                    {
                        forceLoginCallbacks[realm] = callback ?? throw new ArgumentException("The forced login callback must not be null");
                    }

                    /// <summary>
                    ///   This callback method is meant to invoke <see cref="RegisterLoginMethod" />
                    ///   as many times as needed (at least one time will be needed to call).
                    /// </summary>
                    protected abstract void RegisterLoginMethods();

                    /// <summary>
                    ///   Forces a login to a particular account/realm on behalf
                    ///   of another account/realm, typically an administrator
                    ///   one (to debug, or even research a misconduct). This
                    ///   can only be done on server.
                    /// </summary>
                    /// <param name="clientId">The id of the connection that forces the login</param>
                    /// <param name="accountId">The id of the account to force login</param>
                    /// <param name="realm">The realm name of the account to force login</param>
                    /// <param name="forcerAccountId">The id of the account that forces the login</param>
                    /// <param name="forcerRealm">The realm of the account that forces the login</param>
                    /// <returns>A login response</returns>
                    protected Response ForceLogin(ulong clientId, object accountId, string realm, object forcerAccountId, string forcerRealm)
                    {
                        if (!manager.IsServer)
                        {
                            throw new Exception("Forcing a login can only be done in server");
                        }

                        try
                        {
                            Func<object, bool> callback = forceLoginCallbacks[realm];
                            bool accountFound = callback(accountId);
                            if (accountFound)
                            {
                                Response result = new Response() { Success = true, Code = Forced };
                                OnAccountLoginOK(clientId, result, accountId, realm, forcerAccountId, forcerRealm);
                                return result;
                            }
                            else
                            {
                                return new Response() { Success = false, Code = UnknownForcedAccountId };
                            }
                        }
                        catch(KeyNotFoundException)
                        {
                            return new Response() { Success = false, Code = UnknownForcedRealm };
                        }
                    }

                    /// <summary>
                    ///   Attempts an authentication, using a registered
                    ///   message. This can only be done on client. The
                    ///   result is not received in this method, but in
                    ///   the <see cref="OnAuthenticationOK(Response)"/>
                    ///   and <see cref="OnAuthenticationFailure(Response)"/>
                    ///   callbacks that must be implemented.
                    /// </summary>
                    /// <param name="message">The name of the registered authentication message</param>
                    /// <param name="fillRequest">A callback used to populate a network writer with data</param>
                    protected void Authenticate(string message, Action<NetworkWriter> fillRequest)
                    {
                        if (!manager.IsClient)
                        {
                            throw new Exception("Attempting a login can only be done in client");
                        }

                        using (var buffer = PooledNetworkBuffer.Get())
                        using (var writer = PooledNetworkWriter.Get(buffer))
                        {
                            fillRequest(writer);
                            CustomMessagingManager.SendNamedMessage(
                                string.Format("{0}:{1}", LoginMessagePrefix, message),
                                manager.ServerClientId, buffer, NetworkChannel.Internal
                            );
                        }
                    }

                    /// <summary>
                    ///   This method is executed when a successful login is performed, in server side.
                    /// </summary>
                    /// <param name="clientId">The id of the connection that performed the login</param>
                    /// <param name="response">The login response</param>
                    /// <param name="accountId">The id of the account that successfully logged in</param>
                    /// <param name="realm">The realm name of the account that successfully logged in</param>
                    /// <param name="forcerAccountId">The id of the account that forced the login, if this login is forced; null otherwise</param>
                    /// <param name="forcerRealm">The realm name of the account that forced the login, if this login is forced; null otherwise</param>
                    protected abstract void OnAccountLoginOK(ulong clientId, Response response, object accountId, string realm, object forcerAccountId, string forcerRealm);

                    /// <summary>
                    ///   This method is executed when a login was attempted and failed, in server side.
                    /// </summary>
                    /// <param name="clientId">The id of the connection that attempted the login</param>
                    /// <param name="response">The login response</param>
                    /// <param name="accountId">The id of the account whose login was attempted; null if no account was found</param>
                    /// <param name="realm">The realm name of the account whose login was attempted; empty if no account was found</param>
                    protected abstract void OnAccountLoginFailed(ulong clientId, Response response, object accountId, object realm);

                    /// <summary>
                    ///   This method is executed when a successfully login is performed, in client side.
                    /// </summary>
                    /// <param name="response">The received response from server side</param>
                    protected abstract void OnAuthenticationOK(Response response);

                    /// <summary>
                    ///   This method is executed when a login was attempted and failed, in client side.
                    /// </summary>
                    /// <param name="response">The received response from server side</param>
                    protected abstract void OnAuthenticationFailure(Response response);

                    /// <summary>
                    ///   This method is executed when a login was not attempted (timeout), in client side.
                    /// </summary>
                    /// <param name="response">The received response from server side</param>
                    protected abstract void OnAuthenticationTimeout();

                    /// <summary>
                    ///   This method is executed when a login was attempted but the client is already logged in, in client side.
                    /// </summary>
                    /// <param name="response">The received response from server side</param>
                    protected abstract void OnAuthenticationAlreadyDone();

                    /// <summary>
                    ///   Tells whether the given connection is already logged in, or not.
                    /// </summary>
                    /// <param name="clientId">The id of the connection</param>
                    /// <returns>Whether it is already logged in, or not</returns>
                    public abstract bool IsLoggedIn(ulong clientId);
                }
            }
        }
    }
}
