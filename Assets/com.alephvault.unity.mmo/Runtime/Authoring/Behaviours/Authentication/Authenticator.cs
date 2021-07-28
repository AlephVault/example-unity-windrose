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
                    private const string LoggedOut = "__AV:MMO__:LOGGED-OUT";
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

                    // The clients, remote or local, that are undergoing a login or logout process.
                    private HashSet<ulong> authBusyClients = new HashSet<ulong>();

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

                    private void AddAuthBusy(ulong clientId)
                    {
                        Debug.LogFormat("Tagging {0} as auth-busy", clientId);
                        authBusyClients.Add(clientId);
                    }

                    private void RemoveAuthBusy(ulong clientId)
                    {
                        Debug.LogFormat("Untagging {0} as auth-busy", clientId);
                        authBusyClients.Remove(clientId);
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
                        CustomMessagingManager.RegisterNamedMessageHandler(LoggedOut, (senderId, stream) =>
                        {
                            if (manager.IsClient)
                            {
                                using (var reader = PooledNetworkReader.Get(stream))
                                {
                                    Reason reason = new Reason();
                                    reason.NetworkSerialize(reader.Serializer);
                                    OnAuthenticationEnded?.Invoke(reason);
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
                                            // Debug.LogFormat("Disconnecting client {0} due to login timeout", key);
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
                    // If a session exists, it prunes it asynchronously.
                    private void OnClientDisconnectCallback(ulong remoteClientId)
                    {
                        if (manager.IsServer)
                        {
                            AddAuthBusy(remoteClientId);
                            if (sessionByConnectionId.TryGetValue(remoteClientId, out var session))
                            {
                                DoEmergencyLogout(remoteClientId, session);
                            }
                            else
                            {
                                pendingLoginClients.Remove(remoteClientId);
                                RemoveAuthBusy(remoteClientId);
                            }
                        }
                    }

                    private async void DoEmergencyLogout(ulong remoteClientId, Session session)
                    {
                        await TriggerOnAccountLoggedOut(remoteClientId, Reason.LoggedOut, session.Item2);
                        RemoveSession(remoteClientId);
                        pendingLoginClients.Remove(remoteClientId);
                        RemoveAuthBusy(remoteClientId);
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
                                if (IsLoggedInOrAuthBusy(senderId))
                                {
                                    using (var buffer = PooledNetworkBuffer.Get())
                                    {
                                        CustomMessagingManager.SendNamedMessage(AlreadyLoggedIn, senderId, buffer, NetworkChannel.Internal);
                                    }
                                }
                                else
                                {
                                    pendingLoginClients.Remove(senderId);
                                    AddAuthBusy(senderId);
                                    DoAuthenticate(senderId, stream, authenticationMethod);
                                }
                            }
                        });
                    }

                    private async void DoAuthenticate(ulong senderId, System.IO.Stream stream, Func<NetworkReader, Task<Tuple<Response, AccountId>>> authenticationMethod)
                    {
                        Tuple<Response, AccountId> result;
                        using (var reader = PooledNetworkReader.Get(stream))
                        {
                            result = await authenticationMethod(reader);
                        }
                        if (result.Item1.Success)
                        {
                            using (var buffer = PooledNetworkBuffer.Get())
                            using (var writer = PooledNetworkWriter.Get(buffer))
                            {
                                Response response = result.Item1;
                                response.NetworkSerialize(writer.Serializer);
                                AddSession(senderId, result.Item2);
                                CustomMessagingManager.SendNamedMessage(LoginOK, senderId, buffer, NetworkChannel.Internal);
                            }
                            await TriggerOnAccountLoginOK(senderId, result.Item1, result.Item2);
                            RemoveAuthBusy(senderId);
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
                            await TriggerOnAccountLoginFailed(senderId, result.Item1, result.Item2);
                            RemoveAuthBusy(senderId);
                            // For remote clients, disconnect them after a little while.
                            if (!manager.IsClient || senderId != manager.LocalClientId)
                            {
                                delayedTerminator.DelayedDisconnectClient(senderId);
                            }
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
                    ///   Kicks a currently logged account by stating
                    ///   a particular reason. This is only done in
                    ///   server side and, 
                    /// </summary>
                    /// <param name="reason"></param>
                    public void Kick(ulong clientId, Reason reason)
                    {
                        if (!manager.IsServer)
                        {
                            throw new Exception("Attempting a kick can only be done in server");
                        }

                        if (!IsActive(clientId))
                        {
                            throw new Exception("Attempting a kick can only be done on connections that are logged in");
                        }

                        if (manager.ConnectedClients.ContainsKey(clientId) && sessionByConnectionId.TryGetValue(clientId, out var session))
                        {
                            using (var buffer = PooledNetworkBuffer.Get())
                            using (var writer = PooledNetworkWriter.Get(buffer))
                            {
                                reason.NetworkSerialize(writer.Serializer);
                                CustomMessagingManager.SendNamedMessage(LoggedOut, clientId, buffer, NetworkChannel.Internal);
                            }
                            DoKick(clientId, reason, session);
                        }
                    }

                    private async void DoKick(ulong clientId, Reason reason, Session session)
                    {
                        AddAuthBusy(clientId);
                        await TriggerOnAccountLoggedOut(clientId, reason, session.Item2);
                        RemoveSession(clientId);
                        if (!manager.IsClient || manager.LocalClientId != clientId)
                        {
                            delayedTerminator.DelayedDisconnectClient(clientId);
                        }
                        RemoveAuthBusy(clientId);
                    }

                    /// <summary>
                    ///   Issues a graceful logout (i.e. a kick issued
                    ///   by the same user). This method is not meant
                    ///   to be invoked directly by a client without
                    ///   a server-side logic meddling on it. Typically,
                    ///   a server might want a previous cleanup or other
                    ///   steps to be run before invoking tc
                    /// </summary>
                    /// <param name="clientId">The client id asking to logout</param>
                    public void GracefulLogout(ulong clientId)
                    {
                        Kick(clientId, Reason.LoggedOut);
                    }

                    // Triggers, one by one, each async callback in OnAccountLoginOK delegate.
                    private async Task TriggerOnAccountLoginOK(ulong clientId, Response response, AccountId accountId)
                    {
                        if (OnAccountLoginOK != null)
                        {
                            foreach(var callback in OnAccountLoginOK.GetInvocationList())
                            {
                                try
                                {
                                    await ((Func<ulong, Response, AccountId, Task>)callback)(clientId, response, accountId);
                                }
                                catch (Exception e)
                                {
                                    // TODO what to do here?
                                }
                            }
                        }
                    }

                    /// <summary>
                    ///   This event is executed when a successful login is performed, in server side.
                    ///   It accepts asynchronous callbacks.
                    /// </summary>
                    public event Func<ulong, Response, AccountId, Task> OnAccountLoginOK = null;

                    // Triggers, one by one, each async callback in OnAccountLoginFailed delegate.
                    private async Task TriggerOnAccountLoginFailed(ulong clientId, Response response, AccountId accountId)
                    {
                        if (OnAccountLoginFailed != null)
                        {
                            foreach (var callback in OnAccountLoginFailed.GetInvocationList())
                            {
                                try
                                {
                                    await ((Func<ulong, Response, AccountId, Task>)callback)(clientId, response, accountId);
                                }
                                catch (Exception e)
                                {
                                    // TODO what to do here??
                                }
                            }
                        }
                    }

                    /// <summary>
                    ///   This event is executed when a login was attempted and failed, in server side.
                    ///   It accepts asynchronous callbacks.
                    /// </summary>
                    public event Func<ulong, Response, AccountId, Task> OnAccountLoginFailed = null;

                    // Triggers, one by one, each async callback in OnAccountLoggedOut delegate.
                    private async Task TriggerOnAccountLoggedOut(ulong clientId, Reason reason, AccountId accountId)
                    {
                        if (OnAccountLoggedOut != null)
                        {
                            foreach (var callback in OnAccountLoggedOut.GetInvocationList())
                            {
                                try
                                {
                                    await ((Func<ulong, Reason, AccountId, Task>)callback)(clientId, reason, accountId);
                                }
                                catch(Exception e)
                                {
                                    // TODO what to do here??
                                }
                            }
                        }
                    }

                    /// <summary>
                    ///   This event is executed when a logout/kick was done in a logged connection, in server side.
                    ///   Game-logic cleanup will occur on this callback. The session will be alive in the meantime.
                    ///   Game-logic implementations will cleanup everything, and then the connection will be able
                    ///   to be disposed as well. It accepts asynchronous callbacks.
                    /// </summary>
                    public event Func<ulong, Reason, AccountId, Task> OnAccountLoggedOut = null;

                    /// <summary>
                    ///   This event is executed when a successfully login is performed, in client side.
                    /// </summary>
                    public event Action<Response> OnAuthenticationOK = null;

                    /// <summary>
                    ///   This event is executed when a login was attempted and failed, in client side.
                    /// </summary>
                    public event Action<Response> OnAuthenticationFailed = null;

                    /// <summary>
                    ///   This event is executed when a logout/kick was done in the logged connection, in client side.
                    /// </summary>
                    public event Action<Reason> OnAuthenticationEnded = null;

                    /// <summary>
                    ///   This event is executed when a login was not attempted (timeout), in client side.
                    /// </summary>
                    public event Action OnAuthenticationTimeout = null;

                    /// <summary>
                    ///   This method is executed when a login was attempted but the client is already logged in, in client side.
                    /// </summary>
                    public event Action OnAuthenticationAlreadyDone = null;

                    // Tells whether a given connection is logged in.
                    private bool IsLoggedIn(ulong clientId)
                    {
                        return sessionByConnectionId.ContainsKey(clientId);
                    }

                    // Tells whether a given connection is traversing part of the login/logout process.
                    private bool IsAuthBusy(ulong clientId)
                    {
                        return authBusyClients.Contains(clientId);
                    }

                    // Tells whether a given connection is already logged in or at least traversing
                    // part of the login/logout process.
                    private bool IsLoggedInOrAuthBusy(ulong clientId)
                    {
                        return sessionByConnectionId.ContainsKey(clientId) || IsAuthBusy(clientId);
                    }

                    /// <summary>
                    ///   Tells whether the given connection is already logged in, and
                    ///   not busy in neither login/logout. This one, in combination
                    ///   with checking for pending removal, is useful to restrict
                    ///   remote actions from a client.
                    /// </summary>
                    /// <param name="clientId">The id of the connection</param>
                    /// <returns>Whether it is already logged in, or not</returns>
                    public bool IsActive(ulong clientId)
                    {
                        return IsLoggedIn(clientId) && !IsAuthBusy(clientId);
                    }
                }
            }
        }
    }
}
