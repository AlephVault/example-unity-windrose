using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
using UnityEditor.MemoryProfiler;
using NetRose.Behaviours.Sessions.Messages;

namespace NetRose
{
    namespace Behaviours
    {
        namespace Sessions
        {
            /// <summary>
            ///   <para>
            ///     This behaviour is typically a singleton and/or tied
            ///       a <see cref="NetworkWorldManager"/> (it will need
            ///       specifically that type of behaviour as network
            ///       manager) and manages the sessions behind each
            ///       connection to the client (this behaviour only
            ///       makes sense in server side).
            ///   </para>
            ///   <para>
            ///     Sessions will mostly have an autonomous behaviour
            ///       regarding maintenance and termination.
            ///   </para>
            /// </summary>
            /// <typeparam name="AccountID">The type of the id of an account</typeparam>
            /// <typeparam name="AccountData">The type of the data of an account</typeparam>
            /// <typeparam name="CharacterID">The type of the id of a character</typeparam>
            /// <typeparam name="CharacterPreviewData">The type of the partial preview data of a character</typeparam>
            /// <typeparam name="CharacterFullData">The type of the full data of a character</typeparam>
            public abstract class SessionManager<AccountID, AccountData, CharacterID, CharacterPreviewData, CharacterFullData> : MonoBehaviour
            {
                static readonly ILogger logger = LogFactory.GetLogger(typeof(SessionManager<AccountID, AccountData, CharacterID, CharacterPreviewData, CharacterFullData>));

                // The singleton network manager, which must be a world manager.
                private NetworkWorldManager manager;

                // Keeps and retrieves the sessions given their account id.
                private Dictionary<AccountID, Session<AccountID, AccountData, CharacterID, CharacterPreviewData, CharacterFullData>> sessions = new Dictionary<AccountID, Session<AccountID, AccountData, CharacterID, CharacterPreviewData, CharacterFullData>>();

                /// <summary>
                ///   What to do if an account is logging again (i.e. how to handle
                ///     multiple temporary simultaneous copies of the same accounts).
                /// </summary>
                [SerializeField]
                DuplicateAccountRule ResolveDuplicates = DuplicateAccountRule.Kick;

                /// <summary>
                ///   This event carries information of the current session in the server.
                /// </summary>
                public class ServerSessionEvent : UnityEvent<Session<AccountID, AccountData, CharacterID, CharacterPreviewData, CharacterFullData>> {}

                /// <summary>
                ///   This event is triggered when a session started in the server.
                /// </summary>
                public readonly ServerSessionEvent onServerSessionStarted = new ServerSessionEvent();

                /// <summary>
                ///   This event is triggered when a session ended in the server.
                /// </summary>
                public readonly ServerSessionEvent onServerSessionEnded = new ServerSessionEvent();

                /// <summary>
                ///   This event is triggered when, after a successful login, the account id
                ///     fails to be present. Typically, this occurs when an authenticator other
                ///     than the Standard, with compatible types, was used.  A disconnection is
                ///     being issued immediately after.
                /// </summary>
                public readonly UnityEvent onClientAuthMissingAccountID = new UnityEvent();

                /// <summary>
                ///   This event is triggered in the client when a session started in the server.
                /// </summary>
                public readonly UnityEvent onClientSessionStarted = new UnityEvent();

                /// <summary>
                ///   This event is triggered in the client when a session ended in the server.
                ///      A disconnection is being issued immediately after.
                /// </summary>
                public readonly UnityEvent onClientSessionEnded = new UnityEvent();

                /// <summary>
                ///   This event is triggered in the client when an unknown session error occurred in the server.
                ///      A disconnection is being issued immediately after.
                /// </summary>
                public readonly UnityEvent onClientSessionUnknownError = new UnityEvent();

                /// <summary>
                ///   This event is triggered in the client when their current session is ghosted by another login.
                ///     A disconnection is being issued immediately after.
                /// </summary>
                public readonly UnityEvent onClientSessionGhosted = new UnityEvent();

                /// <summary>
                ///   This event is triggered in the client when their current authentication is rolled back due to
                ///     a current session being already established for the attempted user.  A disconnection is being
                ///     issued immediately after.
                /// </summary>
                public readonly UnityEvent onClientSessionDupeKicked = new UnityEvent();

                /// <summary>
                ///   This event is triggered in the client when the server could not find the account data for
                ///     the current user. A disconnection is being issued immediately after.
                /// </summary>
                public readonly UnityEvent onClientSessionMissingAccountData = new UnityEvent();

                /// <summary>
                ///   This event is triggered in the client when the server releases the current in-use character
                ///     for the currently logged-in account.
                /// </summary>
                public readonly UnityEvent onClientSessionReleasingCharacter = new UnityEvent();

                /// <summary>
                ///   This event is triggered in the client when the server attempts to select the only character
                ///     (this message only makes sense for single-character games) but such is not found. It opens
                ///     the opportunity for the client to issue the creation of a character under game-specific
                ///     commands & guidelines.
                /// </summary>
                public readonly UnityEvent onClientSessionNoCharacterAvailable = new UnityEvent();

                /// <summary>
                ///   This event is triggered in the client when the server attempts to select the same character
                ///     that is currently using. This message is an opportunity to tell the client that nothing
                ///     will actually occur.
                /// </summary>
                public readonly UnityEvent onClientSessionAlreadyUsingCharacter = new UnityEvent();

                /// <summary>
                ///   This event is triggered in the client when the server attempts to release the current
                ///     character, but the game is single-character per account. This is typically an error
                ///     in the server implementation rather than a user error (or could be both).
                /// </summary>
                public readonly UnityEvent onClientSessionCannotReleaseCharacterInSingleMode = new UnityEvent();

                /// <summary>
                ///   This event is triggered in the client when the server attempted to release the current
                ///     character being selected, but no current character is actually selected. This message
                ///     only makes sense in multi-character games, and is an opportunity to notify the error
                ///     but the game will not disconnect.
                /// </summary>
                public readonly UnityEvent onClientSessionNotUsingCharacter = new UnityEvent();

                /// <summary>
                ///   This event carries information of the available characters for the account.
                /// </summary>
                public class ChooseCharacterEvent : UnityEvent<IReadOnlyList<Tuple<CharacterID, CharacterPreviewData>>> {}
                
                /// <summary>
                ///   This event is triggered when the client receives a server notification to pick a character.
                ///     The event includes the list of (id, preview data) of each available character.
                /// </summary>
                public readonly ChooseCharacterEvent onClientChooseCharacter = new ChooseCharacterEvent();

                /// <summary>
                ///   This event carries information of the currently in-use character in the account.
                /// </summary>
                public class UsingCharacterEvent : UnityEvent<CharacterID, CharacterFullData> { }

                /// <summary>
                ///   This event is triggered when the client receives a server notification telling that
                ///     a character is currently being selected.
                /// </summary>
                public readonly UsingCharacterEvent onClientUsingCharacter = new UsingCharacterEvent();

                /// <summary>
                ///   This event carries information of the character ID that was deemed of invalid format
                ///     or non-existing per-account value when trying to select a character.
                /// </summary>
                public class BadCharacterIDEvent : UnityEvent<CharacterID> { }

                /// <summary>
                ///   This event is triggered when the client receives a server notification telling that
                ///     an attempted character id is invalid (in format).
                /// </summary>
                public readonly BadCharacterIDEvent onClientInvalidCharacterID = new BadCharacterIDEvent();

                /// <summary>
                ///   This event is triggered when the client receives a server notification telling that
                ///     an attempted character id does not exist for the current account.
                /// </summary>
                public readonly BadCharacterIDEvent onClientCharacterDoesNotExist = new BadCharacterIDEvent();

                protected virtual void Awake()
                {
                    manager = (NetworkManager.singleton as NetworkWorldManager);
                    if (manager == null)
                    {
                        Destroy(gameObject);
                        throw new Exception("The NetworkManager singleton must be of type NetworkWorldManager");
                    }
                    manager.onClientStart.AddListener(SetupClientMessageHandlers);
                    manager.onClientStop.AddListener(TeardownClientMessageHandlers);
                    manager.onConnected.AddListener(OnConnectionStarted);
                    manager.onDisconnected.AddListener(OnConnectionEnded);
                    if ((manager.mode & NetworkManagerMode.ClientOnly) == NetworkManagerMode.ClientOnly) SetupClientMessageHandlers();
                }

                protected virtual void OnDestroy()
                {
                    manager.onClientStart.RemoveListener(SetupClientMessageHandlers);
                    manager.onClientStop.RemoveListener(TeardownClientMessageHandlers);
                    manager.onConnected.RemoveListener(OnConnectionStarted);
                    manager.onDisconnected.RemoveListener(OnConnectionEnded);
                    if ((manager.mode & NetworkManagerMode.ClientOnly) == NetworkManagerMode.ClientOnly) TeardownClientMessageHandlers();
                }

                private void OnConnectionStarted(NetworkConnection connection)
                {
                    if (connection.authenticationData is AccountID)
                    {
                        // To this point, the player does not exist.
                        // Just initialize the sessions without calling
                        // any callback.
                        AccountID accountId = (AccountID)connection.authenticationData;
                        InitSession(connection, accountId);
                    }
                    else
                    {
                        connection.Send(new Messages.MissingAccountID());
                        connection.Disconnect();
                    }
                }

                private void OnConnectionEnded(NetworkConnection connection)
                {
                    // The connection is terminated. We only need to
                    // silently destroy the session.
                    if (connection.authenticationData is AccountID)
                    {
                        AccountID accountId = (AccountID)connection.authenticationData;
                        ClearSession(accountId, true);
                    }
                }

                /***************** Event invokers start here ******************/

                private void OnClientAuthMissingAccountID(MissingAccountID message)
                {
                    onClientAuthMissingAccountID.Invoke();
                }

                private void OnClientSessionStarted(SessionStarted message)
                {
                    onClientSessionStarted.Invoke();
                }

                private void OnClientSessionEnded(SessionEnded message)
                {
                    onClientSessionEnded.Invoke();
                }

                private void OnClientSessionUnknownError(SessionUnknownError message)
                {
                    onClientSessionUnknownError.Invoke();
                }

                private void OnClientSessionGhosted(DupeGhosted message)
                {
                    onClientSessionGhosted.Invoke();
                }

                private void OnClientSessionDupeKicked(DupeKicked message)
                {
                    onClientSessionDupeKicked.Invoke();
                }

                private void OnClientSessionMissingAccountData(MissingAccountData message)
                {
                    onClientSessionMissingAccountData.Invoke();
                }

                private void OnClientSessionReleasingCharacter(ReleasingCharacter message)
                {
                    onClientSessionReleasingCharacter.Invoke();
                }

                private void OnClientSessionNoCharacterAvailable(NoCharacterAvailable message)
                {
                    onClientSessionNoCharacterAvailable.Invoke();
                }

                private void OnClientSessionAlreadyUsingCharacter(AlreadyUsingCharacter message)
                {
                    onClientSessionAlreadyUsingCharacter.Invoke();
                }

                private void OnClientSessionCannotReleaseCharacterInSingleMode(CannotReleaseCharacterInSingleMode message)
                {
                    onClientSessionCannotReleaseCharacterInSingleMode.Invoke();
                }

                private void OnClientSessionNotUsingCharacter(NotUsingCharacter message)
                {
                    onClientSessionNotUsingCharacter.Invoke();
                }

                private void OnClientChooseCharacter<T>(T message) where T : ChooseCharacter<CharacterID, CharacterPreviewData>
                {
                    onClientChooseCharacter.Invoke(message.Characters);
                }

                private void OnClientUsingCharacter<T>(T message) where T : UsingCharacter<CharacterID, CharacterFullData>
                {
                    onClientUsingCharacter.Invoke(message.CurrentCharacterID, message.CurrentCharacterData);
                }

                private void OnClientInvalidCharacterID<T>(T message) where T : InvalidCharacterID<CharacterID>
                {
                    onClientInvalidCharacterID.Invoke(message.ID);
                }

                private void OnClientCharacterDoesNotExist<T>(T message) where T : CharacterDoesNotExist<CharacterID>
                {
                    onClientCharacterDoesNotExist.Invoke(message.ID);
                }

                /***************** Generic events [un]registrar goes here ******************/

                /// <summary>
                ///   This event registers handlers for 4 concrete types of event messages.
                /// </summary>
                /// <typeparam name="CC">The desired subtype of <see cref="ChooseCharacter{CharacterID, CharacterPreviewData}"/> to attend</typeparam>
                /// <typeparam name="UC">The desired subtype of <see cref="UsingCharacter{CharacterID, CharacterFullData}"/> to attend</typeparam>
                /// <typeparam name="IC">The desired subtype of <see cref="InvalidCharacterID{CharacterID}"/> to attend</typeparam>
                /// <typeparam name="NC">The desired subtype of <see cref="CharacterDoesNotExist{CharacterID}"/> to attend</typeparam>
                protected void RegisterCharacterDataEvents<CC, UC, IC, NC>()
                    where CC : ChooseCharacter<CharacterID, CharacterPreviewData>, new()
                    where UC : UsingCharacter<CharacterID, CharacterFullData>, new()
                    where IC : InvalidCharacterID<CharacterID>, new()
                    where NC : CharacterDoesNotExist<CharacterID>, new()
                {
                    NetworkClient.RegisterHandler<CC>(OnClientChooseCharacter, false);
                    NetworkClient.RegisterHandler<UC>(OnClientUsingCharacter, false);
                    NetworkClient.RegisterHandler<IC>(OnClientInvalidCharacterID, false);
                    NetworkClient.RegisterHandler<NC>(OnClientCharacterDoesNotExist, false);
                }

                /// <summary>
                ///   This event unregisters the handlers for 4 concrete types of event messages
                ///     already registered via <see cref="RegisterCharacterDataEvents{CC, UC, IC, NC}"/>.
                /// </summary>
                /// <typeparam name="CC">The desired subtype of <see cref="ChooseCharacter{CharacterID, CharacterPreviewData}"/> to cleanup</typeparam>
                /// <typeparam name="UC">The desired subtype of <see cref="UsingCharacter{CharacterID, CharacterFullData}"/> to cleanup</typeparam>
                /// <typeparam name="IC">The desired subtype of <see cref="InvalidCharacterID{CharacterID}"/> to cleanup</typeparam>
                /// <typeparam name="NC">The desired subtype of <see cref="CharacterDoesNotExist{CharacterID}"/> to cleanup</typeparam>
                protected void UnregisterCharacterDataEvents<CC, UC, IC, NC>()
                    where CC : ChooseCharacter<CharacterID, CharacterPreviewData>, new()
                    where UC : UsingCharacter<CharacterID, CharacterFullData>, new()
                    where IC : InvalidCharacterID<CharacterID>, new()
                    where NC : CharacterDoesNotExist<CharacterID>, new()
                {
                    NetworkClient.UnregisterHandler<CC>();
                    NetworkClient.UnregisterHandler<UC>();
                    NetworkClient.UnregisterHandler<IC>();
                    NetworkClient.UnregisterHandler<NC>();
                }

                /***************** Event invokers end here ******************/

                /// <summary>
                ///   Sets the handlers for some to-client messages.
                ///     This method must be overriden to register
                ///     handlers for the subclasses of the abstract
                ///     generic messages.
                /// </summary>
                protected virtual void SetupClientMessageHandlers()
                {
                    NetworkClient.RegisterHandler<MissingAccountID>(OnClientAuthMissingAccountID, false);
                    NetworkClient.RegisterHandler<SessionStarted>(OnClientSessionStarted, false);
                    NetworkClient.RegisterHandler<SessionEnded>(OnClientSessionEnded, false);
                    NetworkClient.RegisterHandler<SessionUnknownError>(OnClientSessionUnknownError, false);
                    NetworkClient.RegisterHandler<DupeGhosted>(OnClientSessionGhosted, false);
                    NetworkClient.RegisterHandler<DupeKicked>(OnClientSessionDupeKicked, false);
                    NetworkClient.RegisterHandler<MissingAccountData>(OnClientSessionMissingAccountData, false);
                    NetworkClient.RegisterHandler<ReleasingCharacter>(OnClientSessionReleasingCharacter, false);
                    NetworkClient.RegisterHandler<NoCharacterAvailable>(OnClientSessionNoCharacterAvailable, false);
                    NetworkClient.RegisterHandler<AlreadyUsingCharacter>(OnClientSessionAlreadyUsingCharacter, false);
                    NetworkClient.RegisterHandler<CannotReleaseCharacterInSingleMode>(OnClientSessionCannotReleaseCharacterInSingleMode, false);
                    NetworkClient.RegisterHandler<NotUsingCharacter>(OnClientSessionNotUsingCharacter, false);
                    RegisterCharacterDataEvents();
                }

                /// <summary>
                ///   Clears the handlers for some to-client messages.
                /// </summary>
                protected virtual void TeardownClientMessageHandlers()
                {
                    NetworkClient.UnregisterHandler<MissingAccountID>();
                    NetworkClient.UnregisterHandler<SessionStarted>();
                    NetworkClient.UnregisterHandler<SessionEnded>();
                    NetworkClient.UnregisterHandler<SessionUnknownError>();
                    NetworkClient.UnregisterHandler<DupeGhosted>();
                    NetworkClient.UnregisterHandler<DupeKicked>();
                    NetworkClient.UnregisterHandler<MissingAccountData>();
                    NetworkClient.UnregisterHandler<ReleasingCharacter>();
                    NetworkClient.UnregisterHandler<NoCharacterAvailable>();
                    NetworkClient.UnregisterHandler<AlreadyUsingCharacter>();
                    NetworkClient.UnregisterHandler<CannotReleaseCharacterInSingleMode>();
                    NetworkClient.UnregisterHandler<NotUsingCharacter>();
                    UnregisterCharacterDataEvents();
                }

                /***********************************************************************************/
                /*********** Methods of account/character(s) data-fetching *************************/
                /*********** and serialization to the client(s). Also one  *************************/
                /*********** method to fill with a call to the generic one *************************/
                /*********** to register and unregister data-aware events. *************************/
                /***********************************************************************************/

                /// <summary>
                ///   This method must be implemented by calling an instance of the generic
                ///     version of this method: RegisterCharacterDataEvents<CC, UC, IC, NC>();
                /// </summary>
                protected abstract void RegisterCharacterDataEvents();

                /// <summary>
                ///   This method must be implemented by calling an instance of the generic
                ///     version of this method: UnregisterCharacterDataEvents<CC, UC, IC, NC>();
                /// </summary>
                protected abstract void UnregisterCharacterDataEvents();

                /// <summary>
                ///   An account data fetcher is used to get the account's data by the given
                ///     ID. Errors may be freely thrown from this getter.
                /// </summary>
                protected abstract Contracts.AccountFetcher<AccountID, AccountData> GetAccountFetcher();

                /// <summary>
                ///   Retrieves the account data. If the result is <c>default(AccountID)</c>,
                ///     it will be considered as if the account does not exist. That result
                ///     is typically achieved when invoking this method from a public scope,
                ///     and/or if the account was deleted in the mean time. Errors can freely
                ///     be thrown, but they will go blind when this happens in the session
                ///     workflow. This task is asynchronous and must be waited for.
                /// </summary>
                /// <param name="accountId">The account ID for the lookup</param>
                /// <returns>The top-level data of the account (e.g. username, profile data, ...)</returns>
                public Task<AccountData> GetAccountData(AccountID accountId)
                {
                    return GetAccountFetcher().GetAccountData(accountId);
                }

                /// <summary>
                ///   An account character data fetcher is used to get the account characters'
                ///     data, as list/preview and as one/full data.
                /// </summary>
                protected abstract Contracts.AccountCharacterFetcher<AccountID, CharacterID, CharacterPreviewData, CharacterFullData> GetAccountCharacterFetcher();

                /// <summary>
                ///   Retrieves an account's character data. If the result is
                ///     <c>default(CharacterFullData)</c> then it may be because
                ///     either the character or the account do not exist. Errors
                ///     can freely be thrown, but they will go blind when this
                ///     happens in the session workflow. This task is asynchronous
                ///     and must be waited for.
                /// </summary>
                /// <param name="accountId">The account ID for the lookup</param>
                /// <param name="characterId">The id of the character to get the full data from</param>
                /// <returns>The full data of the retrieved character</returns>
                public Task<CharacterFullData> GetCharacterData(AccountID accountId, CharacterID characterId)
                {
                    return GetAccountCharacterFetcher().GetCharacterData(accountId, characterId);
                }

                /// <summary>
                ///   Retrieves the account's characters preview data. The result
                ///     may be empty. Errors can freely be thrown, but they will
                ///     go blind when this happens in the session workfow. This
                ///     task is asynchronous and must be waited for.
                /// </summary>
                /// <param name="accountId">The account ID for the lookup</param>
                /// <returns>The list of available characters as (id, data) pairs</returns>
                public Task<List<Tuple<CharacterID, CharacterPreviewData>>> ListCharacters(AccountID accountId)
                {
                    return GetAccountCharacterFetcher().ListCharacters(accountId);
                }

                /// <summary>
                ///   Retrieves whether the underlying accounts system supports
                ///     multiple characters per account or just one.
                /// </summary>
                public bool AccountsHaveMultipleCharacters()
                {
                    return GetAccountCharacterFetcher().AccountsHaveMultipleCharacters();
                }

                /// <summary>
                ///   Builds a custom message to send the characters list to the
                ///     client side.
                /// </summary>
                /// <param name="characters">The characters to serialize</param>
                /// <returns>The message to send</returns>
                public abstract ChooseCharacter<CharacterID, CharacterPreviewData> MakeChooseCharacterMessage(IReadOnlyList<Tuple<CharacterID, CharacterPreviewData>> characters);

                /// <summary>
                ///   Builds a custom message to send the currently selected
                ///     character's full data.
                /// </summary>
                /// <param name="characterId">The id to serialize</param>
                /// <param name="characterFullData">The full data to serialize</param>
                /// <returns>The message to send</returns>
                public abstract UsingCharacter<CharacterID, CharacterFullData> MakeCurrentCharacterMessage(CharacterID characterId, CharacterFullData characterFullData);

                /// <summary>
                ///   Builds a custom message to send that a certain character id
                ///     is invalid.
                /// </summary>
                /// <param name="characterId">The id to serialize</param>
                /// <returns>The message to send</returns>
                public abstract InvalidCharacterID<CharacterID> MakeInvalidCharacterMessage(CharacterID characterId);

                /// <summary>
                ///   Builds a custom message to send that a certain character id
                ///     does not exist for a given account.
                /// </summary>
                /// <param name="characterId">The id to serialize</param>
                /// <returns>The message to send</returns>
                public abstract CharacterDoesNotExist<CharacterID> MakeNonExistingCharacterMessage(CharacterID characterId);

                /***********************************************************************************/
                /***********************************************************************************/
                /***********************************************************************************/

                private async void InitSession(NetworkConnection connection, AccountID accountId)
                {
                    // If there is a connection with the same account id, it must
                    // kick either of the connections. If the new connection is
                    // not kicked (due to ghost mode, or to not having duplicate
                    // logins) then the process must start for the session.
                    Session<AccountID, AccountData, CharacterID, CharacterPreviewData, CharacterFullData> currentSession;
                    if (sessions.TryGetValue(accountId, out currentSession))
                    {
                        if (ResolveDuplicates == DuplicateAccountRule.Kick)
                        {
                            connection.Send(new Messages.DupeKicked());
                            connection.authenticationData = null;
                            connection.Disconnect();
                        }
                        else // DuplicateAccountRule.Ghost
                        {
                            connection.Send(new Messages.DupeGhosted());
                            currentSession.Connection.Disconnect();
                            await InstallAndLaunchSession(connection, accountId);
                        }
                    }
                    else
                    {
                        await InstallAndLaunchSession(connection, accountId);
                    }
                }

                // Installs and launches the newly-approved session.
                private async Task InstallAndLaunchSession(NetworkConnection connection, AccountID accountId)
                {
                    Session<AccountID, AccountData, CharacterID, CharacterPreviewData, CharacterFullData> session = new Session<AccountID, AccountData, CharacterID, CharacterPreviewData, CharacterFullData>(
                        this, connection, accountId
                    );
                    sessions.Add(accountId, session);
                    connection.Send(new Messages.SessionStarted()); // Will trigger: session started (client side).
                    onServerSessionStarted.Invoke(session);
                    await session.Reset();
                }

                // Clearing the session is only needed INSIDE this class.
                // Returns the cleared session, if any.
                private Session<AccountID, AccountData, CharacterID, CharacterPreviewData, CharacterFullData> ClearSession(AccountID accountId, bool alreadyDisconnected)
                {
                    Session<AccountID, AccountData, CharacterID, CharacterPreviewData, CharacterFullData> outSession;
                    if (sessions.TryGetValue(accountId, out outSession))
                    {
                        outSession.ClearListeners();
                        if (!alreadyDisconnected)
                        {
                            outSession.Connection.Send(new Messages.SessionEnded()); // Will trigger: session ended (client side).
                            //   - Clients must process the "session ended" when they receive this message, or
                            //     when they disconnect, if they did not receive this message just before.
                        }
                        onServerSessionEnded.Invoke(outSession);
                        sessions.Remove(accountId);
                    }
                    return outSession;
                }

                /// <summary>
                ///   Forcefully closes a session and sends the reason to the client.
                /// </summary>
                /// <typeparam name="T">The message type</typeparam>
                /// <param name="accountId">The ID of the session</param>
                /// <param name="message">The nessage to send to the client</param>
                /// <returns>Whether a session was found (and kicked)</returns>
                public bool Kick<T>(AccountID accountId, T message) where T : IMessageBase
                {
                    Session<AccountID, AccountData, CharacterID, CharacterPreviewData, CharacterFullData> session = ClearSession(accountId, false);
                    if (session != null)
                    {
                        session.Connection.Send<T>(message);
                        session.Connection.Disconnect();
                        return true;
                    }
                    return false;
                }

                /// <summary>
                ///   Tells whether a session is active in this manager.
                /// </summary>
                /// <param name="session">The session to check</param>
                /// <returns>Whether it is active or not</returns>
                public bool HasSession(Session<AccountID, AccountData, CharacterID, CharacterPreviewData, CharacterFullData> session)
                {
                    Session<AccountID, AccountData, CharacterID, CharacterPreviewData, CharacterFullData> outSession;
                    return sessions.TryGetValue(session.AccountID, out outSession) && outSession == session;
                }
            }
        }
    }
}
