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
                using AlephVault.Unity.MMO.Types.Authentication;
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
                    ///   The current login status of the connection.
                    /// </summary>
                    public enum Status
                    {
                        Unlogged,
                        Logging,
                        Logged,
                        Unlogging
                    }

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
                    private const string UnexpectedError = "__AV:MMO__:ERROR:UNEXPECTED";

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

                    // Sessions will be tracked by the connection they belong to.
                    private SessionByConnectionId sessionByConnectionId = new SessionByConnectionId();

                    // There will be one bag per status, to keep all the connections.
                    // That bag will also be a dictionary, keeping one timer for
                    // each connection in that status (the timer is only actually
                    // used in the Unlogged status for non-host clients so far).
                    private Dictionary<Status, Dictionary<ulong, uint>> connectionsInStatus = new Dictionary<Status, Dictionary<ulong, uint>>()
                    {
                        { Status.Unlogged, new Dictionary<ulong, uint>() },
                        { Status.Logging, new Dictionary<ulong, uint>() },
                        { Status.Logged, new Dictionary<ulong, uint>() },
                        { Status.Unlogging, new Dictionary<ulong, uint>() },
                    };

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

                    /// <summary>
                    ///   Tells whether a session exists for a given connection.
                    /// </summary>
                    /// <param name="clientId">The connection whose session is told to exist or not</param>
                    /// <returns>Whether the session exists or not</returns>
                    public bool SessionExists(ulong clientId)
                    {
                        return sessionByConnectionId.ContainsKey(clientId);
                    }

                    /// <summary>
                    ///   Sets a given data value in the session for a given connection.
                    /// </summary>
                    /// <param name="clientId">The connection whose session is to be affected</param>
                    /// <param name="key">The in-session key</param>
                    /// <param name="value">The new value</param>
                    public void SetSessionData(ulong clientId, string key, object value)
                    {
                        Session session;

                        try
                        {
                            session = sessionByConnectionId[clientId];
                        }
                        catch(KeyNotFoundException)
                        {
                            throw new Exception("Trying to access a missing session");
                        }

                        session.Item3[key] = value;
                    }

                    /// <summary>
                    ///   Gets a given data value in the session from a given connection.
                    /// </summary>
                    /// <param name="clientId">The connection whose session is to be queried</param>
                    /// <param name="key">The in-session key</param>
                    /// <returns>The session value</returns>
                    /// <remarks>Throws a KeyNotFound error for a missing session key</remarks>
                    public object GetSessionData(ulong clientId, string key)
                    {
                        Session session;

                        try
                        {
                            session = sessionByConnectionId[clientId];
                        }
                        catch (KeyNotFoundException)
                        {
                            throw new Exception("Trying to access a missing session");
                        }

                        return session.Item3[key];
                    }

                    /// <summary>
                    ///   Tries to get a given data value in the session for a given connection.
                    /// </summary>
                    /// <param name="clientId">The connection whose session is to be queried</param>
                    /// <param name="key">The in-session key</param>
                    /// <param name="data">The data to be retrieved</param>
                    /// <returns>Whether the key existed and data was retrieved</returns>
                    public bool TryGetSessionData(ulong clientId, string key, out object data)
                    {
                        Session session;

                        try
                        {
                            session = sessionByConnectionId[clientId];
                        }
                        catch (KeyNotFoundException)
                        {
                            throw new Exception("Trying to access a missing session");
                        }

                        return session.Item3.TryGetValue(key, out data);
                    }

                    /// <summary>
                    ///   Removes a given data value in the session for a given connection.
                    /// </summary>
                    /// <param name="clientId">The connection whose session is to be affected</param>
                    /// <param name="key">The in-session key to be removed</param>
                    /// <returns>Whether that key was removed or not</returns>
                    public bool RemoveSessionData(ulong clientId, string key)
                    {
                        Session session;

                        try
                        {
                            session = sessionByConnectionId[clientId];
                        }
                        catch (KeyNotFoundException)
                        {
                            throw new Exception("Trying to access a missing session");
                        }

                        return session.Item3.Remove(key);
                    }

                    /// <summary>
                    ///   Clears all the session entries in its data.
                    /// </summary>
                    /// <param name="clientId">The connection whose session is to be affected</param>
                    /// <param name="userDataOnly">Whether to remove only the user-defined entries, or the whole session data</param>
                    public void ClearSessionUserData(ulong clientId, bool userDataOnly = true)
                    {
                        Session session;

                        try
                        {
                            session = sessionByConnectionId[clientId];
                        }
                        catch (KeyNotFoundException)
                        {
                            throw new Exception("Trying to access a missing session");
                        }

                        if (userDataOnly)
                        {
                            session.Item3.ClearUserEntries();
                        }
                        else
                        {
                            session.Item3.Clear();
                        }
                    }

                    /// <summary>
                    ///   Tells whether the session data contains a particular key.
                    /// </summary>
                    /// <param name="clientId">The connection whose session is to be queried</param>
                    /// <param name="key">The in-session key</param>
                    /// <returns></returns>
                    public bool SessionContainsKey(ulong clientId, string key)
                    {
                        Session session;

                        try
                        {
                            session = sessionByConnectionId[clientId];
                        }
                        catch (KeyNotFoundException)
                        {
                            throw new Exception("Trying to access a missing session");
                        }

                        return session.Item3.ContainsKey(key);
                    }

                    private void ClearStatus(ulong connectionId)
                    {
                        foreach (Dictionary<ulong, uint> connections in connectionsInStatus.Values)
                        {
                            connections.Remove(connectionId);
                        }
                    }

                    private void SetStatus(ulong connectionId, Status status)
                    {
                        ClearStatus(connectionId);
                        Debug.LogFormat("Setting status for connection {0} to {1}", connectionId, status);
                        connectionsInStatus[status].Add(connectionId, 0);
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
                                foreach(ulong key in connectionsInStatus[Status.Unlogged].Keys)
                                {
                                    Debug.LogFormat("Key in pending bag: {0}", key);
                                    if (connectionsInStatus[Status.Unlogged][key] >= pendingLoginTimeout)
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
                                    connectionsInStatus[Status.Unlogged][key] += 1;
                                }
                            }
                        }
                    }

                    // Adds the remote client to the login-pending list.
                    private void OnClientConnectedCallback(ulong remoteClientId)
                    {
                        if (manager.IsServer)
                        {
                            SetStatus(remoteClientId, Status.Unlogged);
                        }
                    }

                    // Removes the remote client from the login-pending list.
                    // If a session exists, it prunes it asynchronously.
                    private void OnClientDisconnectCallback(ulong remoteClientId)
                    {
                        if (manager.IsServer)
                        {
                            SetStatus(remoteClientId, Status.Unlogging);
                            if (sessionByConnectionId.TryGetValue(remoteClientId, out var session))
                            {
                                DoEmergencyLogout(remoteClientId, session);
                            }
                            else
                            {
                                ClearStatus(remoteClientId);
                            }
                        }
                    }

                    private async void DoEmergencyLogout(ulong remoteClientId, Session session)
                    {
                        await TriggerOnAccountLoggedOut(remoteClientId, Reason.LoggedOut, session.Item2);
                        RemoveSession(remoteClientId);
                        ClearStatus(remoteClientId);
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
                                if (!connectionsInStatus[Status.Unlogged].ContainsKey(senderId))
                                {
                                    using (var buffer = PooledNetworkBuffer.Get())
                                    {
                                        CustomMessagingManager.SendNamedMessage(AlreadyLoggedIn, senderId, buffer, NetworkChannel.Internal);
                                    }
                                }
                                else
                                {
                                    SetStatus(senderId, Status.Logging);
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
                            try
                            {
                                await TriggerOnAccountLoginOK(senderId, result.Item1, result.Item2);
                                SetStatus(senderId, Status.Logged);
                            }
                            catch (LoginAborted e)
                            {
                                // This status will last what a fart in a basket.
                                SetStatus(senderId, Status.Logged);
                                // TODO log the error `e`.
                                Kick(senderId, new Reason() { Graceful = false, Code = e.Code, Text = e.Message });
                            }
                            catch (Exception e)
                            {
                                // This status will last what a fart in a basket.
                                SetStatus(senderId, Status.Logged);
                                // TODO log the error `e`.
                                Kick(senderId, new Reason() { Graceful = false, Code = UnexpectedError, Text = "Unexpected error on login" });
                            }
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
                            SetStatus(senderId, Status.Unlogged);
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
                        SetStatus(clientId, Status.Unlogging);
                        await TriggerOnAccountLoggedOut(clientId, reason, session.Item2);
                        RemoveSession(clientId);
                        SetStatus(clientId, Status.Unlogged);
                        if (!manager.IsClient || manager.LocalClientId != clientId)
                        {
                            delayedTerminator.DelayedDisconnectClient(clientId);
                        }
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
                                await ((Func<ulong, Response, AccountId, Task>)callback)(clientId, response, accountId);
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
                                    // TODO what to do here?? Perhaps logging the exception.
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
                                    // TODO what to do here?? Perhaps logging the exception.
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
                        return connectionsInStatus[Status.Logged].ContainsKey(clientId);
                    }
                }
            }
        }
    }
}
