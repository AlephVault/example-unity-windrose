using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using MLAPI;

namespace AlephVault.Unity.MMO
{
    namespace Authoring
    {
        namespace Behaviours
        {
            namespace Authentication
            {
                using AlephVault.Unity.Support.Utils;
                using MLAPI.Connection;
                using MLAPI.Messaging;
                using MLAPI.Serialization.Pooled;
                using MLAPI.Transports;
                using Types;

                [RequireComponent(typeof(NetworkManager))]
                public abstract class Authenticator : MonoBehaviour
                {
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
                            if (manager.IsClient)
                            {
                                OnAuthenticationAlreadyDone();
                            }
                        });
                        CustomMessagingManager.RegisterNamedMessageHandler(LoginTimeout, (senderId, stream) =>
                        {
                            if (manager.IsClient)
                            {
                                OnAuthenticationTimeout();
                            }
                        });
                        CustomMessagingManager.RegisterNamedMessageHandler(LoginOK, (senderId, stream) =>
                        {
                            if (manager.IsClient)
                            {
                                using (var buffer = PooledNetworkBuffer.Get())
                                using (var reader = PooledNetworkReader.Get(buffer))
                                {
                                    Response response = new Response();
                                    response.NetworkSerialize(reader.Serializer);
                                    OnAuthenticationOK(response);
                                }
                            }
                        });
                        CustomMessagingManager.RegisterNamedMessageHandler(LoginFailed, (senderId, stream) =>
                        {
                            if (manager.IsClient)
                            {
                                using (var buffer = PooledNetworkBuffer.Get())
                                using (var reader = PooledNetworkReader.Get(buffer))
                                {
                                    Response response = new Response();
                                    response.NetworkSerialize(reader.Serializer);
                                    OnAuthenticationFailure(response);
                                }
                            }
                        });
                        RegisterCustomLoginMessages();
                    }

                    // On update, it runs the interval and checks the
                    // clients that are timed-out on pending login,
                    // and kicks them.
                    private void Update()
                    {
                        currentSecondFraction += Time.unscaledDeltaTime;
                        if (currentSecondFraction >= 1)
                        {
                            currentSecondFraction -= 1;
                            foreach(var pair in pendingLoginClients)
                            {
                                if (pair.Value >= pendingLoginTimeout)
                                {
                                    using (var buffer = PooledNetworkBuffer.Get())
                                    {
                                        CustomMessagingManager.SendNamedMessage(LoginTimeout, pair.Key, buffer, NetworkChannel.Internal);
                                        manager.DisconnectClient(pair.Key);
                                    }
                                }
                                else
                                {
                                    pendingLoginClients[pair.Key] += 1;
                                }
                            }
                        }
                    }

                    // Adds the remote client to the login-pending list.
                    private void OnClientDisconnectCallback(ulong remoteClientId)
                    {
                        pendingLoginClients.Add(remoteClientId, 0);
                    }

                    // Removes the remote client from the login-pending list.
                    private void OnClientConnectedCallback(ulong remoteClientId)
                    {
                        pendingLoginClients.Remove(remoteClientId);
                    }

                    /// <summary>
                    ///   <para>
                    ///     Registers a new server-side handler to perform a login
                    ///     using a particular implementation that expects particular
                    ///     data in the stream mode and also returns a particular
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
                    protected void RegisterLoginMethod(string messageName, Func<System.IO.Stream, Task<Tuple<Response, object, string>>> authenticationMethod)
                    {
                        CustomMessagingManager.RegisterNamedMessageHandler(messageName, (senderId, stream) =>
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
                                    Task<Tuple<Response, object, string>> task = authenticationMethod(stream);
                                    task.GetAwaiter().OnCompleted(() =>
                                    {
                                        Tuple<Response, object, string> result = task.Result;
                                        if (result.Item1.Success)
                                        {
                                            pendingLoginClients.Remove(senderId);
                                            OnAccountLoginOK(result.Item1, result.Item2, result.Item3, null, "");
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
                                            OnAccountLoginFailed(result.Item1, result.Item2, result.Item3);
                                            using (var buffer = PooledNetworkBuffer.Get())
                                            using (var writer = PooledNetworkWriter.Get(buffer))
                                            {
                                                Response response = result.Item1;
                                                response.NetworkSerialize(writer.Serializer);
                                                CustomMessagingManager.SendNamedMessage(LoginFailed, senderId, buffer, NetworkChannel.Internal);
                                            }
                                            // For remote clients, disconnect them instantly.
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
                    protected abstract void RegisterCustomLoginMessages();

                    /// <summary>
                    ///   Forces a login to a particular account/realm on behalf
                    ///   of another account/realm, typically an administrator
                    ///   one (to debug, or even research a misconduct).
                    /// </summary>
                    /// <param name="accountId">The id of the account to force login</param>
                    /// <param name="realm">The realm name of the account to force login</param>
                    /// <param name="forcerAccountId">The id of the account that forces the login</param>
                    /// <param name="forcerRealm">The realm of the account that forces the login</param>
                    /// <returns>A login response</returns>
                    public Response ForceLogin(object accountId, string realm, object forcerAccountId, string forcerRealm)
                    {
                        try
                        {
                            Func<object, bool> callback = forceLoginCallbacks[realm];
                            bool accountFound = callback(accountId);
                            if (accountFound)
                            {
                                Response result = new Response() { Success = true, Code = Forced };
                                OnAccountLoginOK(result, accountId, realm, forcerAccountId, forcerRealm);
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
                    ///   This method is executed when a successful login is performed, in server side.
                    /// </summary>
                    /// <param name="response">The login response</param>
                    /// <param name="accountId">The id of the account that successfully logged in</param>
                    /// <param name="realm">The realm name of the account that successfully logged in</param>
                    /// <param name="forcerAccountId">The id of the account that forced the login, if this login is forced; null otherwise</param>
                    /// <param name="forcerRealm">The realm name of the account that forced the login, if this login is forced; null otherwise</param>
                    protected abstract void OnAccountLoginOK(Response response, object accountId, string realm, object forcerAccountId, string forcerRealm);

                    /// <summary>
                    ///   This method is executed when a login was attempted and failed, in server side.
                    /// </summary>
                    /// <param name="response">The login response</param>
                    /// <param name="accountId">The id of the account whose login was attempted; null if no account was found</param>
                    /// <param name="realm">The realm name of the account whose login was attempted; empty if no account was found</param>
                    protected abstract void OnAccountLoginFailed(Response response, object accountId, object realm);

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
