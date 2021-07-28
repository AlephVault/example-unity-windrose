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

                /// <summary>
                ///   Authenticators provide a way to register custom
                ///   login lifecycles (and only login - not signup),
                ///   considering the handling of login responses and
                ///   eventual timeouts during the login handshake
                ///   process. Both client-side and server-side
                ///   callbacks are provided for both success and
                ///   failure during that handshake.
                /// </summary>
                [RequireComponent(typeof(DelayedRemoteClientTerminator))]
                public class Authenticator : MonoBehaviour
                {
                    /// <summary>
                    ///   An account ID has both the ID to use, and the
                    ///   realm the ID belongs to.
                    /// </summary>
                    public class AccountId : Tuple<object, string>
                    {
                        /// <summary>
                        ///   Initializes the account id with the required arguments.
                        /// </summary>
                        /// <param name="item1">The ID of the account</param>
                        /// <param name="item2">The realm of the account</param>
                        public AccountId(object item1, string item2) : base(item1, item2) {}
                    }

                    // The session data is just a mapping of keys and values.
                    // An additional feature of this class is that it allows
                    // clearing everything but the keys starting with __AV:MMO__.
                    private class SessionData : Dictionary<string, object> {
                        /// <summary>
                        ///   Clears the user-defined keys.
                        /// </summary>
                        public void ClearUserEntries()
                        {
                            List<string> keys = (from key in Keys where !key.StartsWith("__AV:MMO__") select key).ToList();
                            foreach(string key in keys)
                            {
                                Remove(key);
                            }
                        }
                    }

                    // A session holds session data, a connection id, and an
                    // account id. The data will only be accessible here -in
                    // the authenticator- but also indirectly (via wrapper
                    // methods) to the outside world.
                    private class Session : Tuple<ulong, AccountId, SessionData> {
                        /// <summary>
                        ///   Instantiates the tuple with a default value: an empty,
                        ///   yet not null, session data dictionary.
                        /// </summary>
                        /// <param name="connectionId">The ID of the connection</param>
                        /// <param name="accountId">The full ID/Realm of the account</param>
                        public Session(ulong connectionId, AccountId accountId) : base(connectionId, accountId, new SessionData()) {}
                    }

                    // A session mapped by its connection id.
                    private class SessionByConnectionId : Dictionary<ulong, Session> {}

                    private const string LoginMessagePrefix = "__AV:MMO__:LOGIN:DO";
                    private const string LoginOK = "__AV:MMO__:LOGIN:OK";
                    private const string LoginFailed = "__AV:MMO__:LOGIN:FAILED";
                    private const string AlreadyLoggedIn = "__AV:MMO__:LOGIN:ALREADY";
                    private const string LoginTimeout = "__AV:MMO__:LOGIN:TIMEOUT";

                    private DelayedRemoteClientTerminator delayedTerminator;
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

                    // A mutex to interact with the sessions.
                    private Mutex mutex = new Mutex();

                    // Sessions will be tracked by the connection they belong to.
                    private SessionByConnectionId sessionByConnectionId = new SessionByConnectionId();

                    // Creates a new session (and adds it to the internal dictionary).
                    private Session AddSession(ulong clientId, AccountId accountId)
                    {
                        Session session = new Session(clientId, accountId);
                        sessionByConnectionId.Add(clientId, session);
                        return session;
                    }

                    // Removes a session by its connection id.
                    private bool RemoveSession(ulong clientId)
                    {
                        return sessionByConnectionId.Remove(clientId);
                    }

                    private void Awake()
                    {
                        manager = GetComponent<NetworkManager>();
                        delayedTerminator = GetComponent<DelayedRemoteClientTerminator>();
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
                                OnAuthenticationAlreadyDone?.Invoke();
                            }
                        });
                        CustomMessagingManager.RegisterNamedMessageHandler(LoginTimeout, (senderId, stream) =>
                        {
                            if (manager.IsClient)
                            {
                                OnAuthenticationTimeout?.Invoke();
                            }
                        });
                        CustomMessagingManager.RegisterNamedMessageHandler(LoginOK, (senderId, stream) =>
                        {
                            if (manager.IsClient)
                            {
                                using (var reader = PooledNetworkReader.Get(stream))
                                {
                                    Response response = new Response();
                                    response.NetworkSerialize(reader.Serializer);
                                    OnAuthenticationOK?.Invoke(response);
                                }
                            }
                        });
                        CustomMessagingManager.RegisterNamedMessageHandler(LoginFailed, (senderId, stream) =>
                        {
                            if (manager.IsClient)
                            {
                                using (var reader = PooledNetworkReader.Get(stream))
                                {
                                    Response response = new Response();
                                    response.NetworkSerialize(reader.Serializer);
                                    OnAuthenticationFailed?.Invoke(response);
                                }
                            }
                        });
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
                        }
                    }

                    // Removes the remote client from the login-pending list.
                    private void OnClientDisconnectCallback(ulong remoteClientId)
                    {
                        if (manager.IsServer)
                        {
                            RemoveSession(remoteClientId);
                            pendingLoginClients.Remove(remoteClientId);
                        }
                    }

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
                    public void RegisterLoginMethod(string messageName, Func<NetworkReader, Task<Tuple<Response, AccountId>>> authenticationMethod)
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
                                    Task<Tuple<Response, AccountId>> task = null;
                                    using (var reader = PooledNetworkReader.Get(stream))
                                    {
                                        task = authenticationMethod(reader);
                                    }

                                    task.GetAwaiter().OnCompleted(() =>
                                    {
                                        Tuple<Response, AccountId> result = task.Result;
                                        if (result.Item1.Success)
                                        {
                                            pendingLoginClients.Remove(senderId);
                                            using (var buffer = PooledNetworkBuffer.Get())
                                            using (var writer = PooledNetworkWriter.Get(buffer))
                                            {
                                                Response response = result.Item1;
                                                response.NetworkSerialize(writer.Serializer);
                                                AddSession(senderId, result.Item2);
                                                CustomMessagingManager.SendNamedMessage(LoginOK, senderId, buffer, NetworkChannel.Internal);
                                            }
                                            OnAccountLoginOK?.Invoke(senderId, result.Item1, result.Item2);
                                        }
                                        else
                                        {
                                            using (var buffer = PooledNetworkBuffer.Get())
                                            using (var writer = PooledNetworkWriter.Get(buffer))
                                            {
                                                Response response = result.Item1;
                                                response.NetworkSerialize(writer.Serializer);
                                                CustomMessagingManager.SendNamedMessage(LoginFailed, senderId, buffer, NetworkChannel.Internal);
                                            }
                                            OnAccountLoginFailed?.Invoke(senderId, result.Item1, result.Item2);
                                            // For remote clients, disconnect them after a little while.
                                            if (!manager.IsClient || senderId != manager.LocalClientId)
                                            {
                                                delayedTerminator.DelayedDisconnectClient(senderId);
                                            }
                                        }
                                    });
                                }
                            }
                        });
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
                    public void Authenticate(string message, Action<NetworkWriter> fillRequest)
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
                    /// <param name="accountId">The ID/Realm of the account that successfully logged in</param>
                    public event Action<ulong, Response, AccountId> OnAccountLoginOK = null;

                    /// <summary>
                    ///   This method is executed when a login was attempted and failed, in server side.
                    /// </summary>
                    /// <param name="clientId">The id of the connection that attempted the login</param>
                    /// <param name="response">The login response</param>
                    /// <param name="accountId">The ID/Realm of the account that attempted a login</param>
                    public event Action<ulong, Response, AccountId> OnAccountLoginFailed = null;

                    /// <summary>
                    ///   This event is executed when a successfully login is performed, in client side.
                    /// </summary>
                    /// <param name="response">The received response from server side</param>
                    public event Action<Response> OnAuthenticationOK = null;

                    /// <summary>
                    ///   This event is executed when a login was attempted and failed, in client side.
                    /// </summary>
                    /// <param name="response">The received response from server side</param>
                    public event Action<Response> OnAuthenticationFailed = null;

                    /// <summary>
                    ///   This event is executed when a login was not attempted (timeout), in client side.
                    /// </summary>
                    public event Action OnAuthenticationTimeout = null;

                    /// <summary>
                    ///   This method is executed when a login was attempted but the client is already logged in, in client side.
                    /// </summary>
                    public event Action OnAuthenticationAlreadyDone = null;

                    /// <summary>
                    ///   Tells whether the given connection is already logged in, or not.
                    ///   This checks whether a session exists.
                    /// </summary>
                    /// <param name="clientId">The id of the connection</param>
                    /// <returns>Whether it is already logged in, or not</returns>
                    public bool IsLoggedIn(ulong clientId)
                    {
                        return sessionByConnectionId.ContainsKey(clientId);
                    }
                }
            }
        }
    }
}
