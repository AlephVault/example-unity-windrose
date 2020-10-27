using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
using UnityEditor.MemoryProfiler;

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
                ///   This event is triggered when a session started in the client.
                /// </summary>
                public readonly UnityEvent onClientSessionStarted = new UnityEvent();

                /// <summary>
                ///   This event is triggered when a session ended in the client.
                /// </summary>
                public readonly UnityEvent onClientSessionEnded = new UnityEvent();

                protected virtual void Awake()
                {
                    manager = (NetworkManager.singleton as NetworkWorldManager);
                    if (manager == null)
                    {
                        Destroy(gameObject);
                        throw new Exception("The NetworkManager singleton must be of type NetworkWorldManager");
                    }
                    manager.onServerStart.AddListener(SetupServerMessageHandlers);
                    manager.onClientStart.AddListener(SetupClientMessageHandlers);
                    manager.onServerStop.AddListener(TeardownServerMessageHandlers);
                    manager.onClientStop.AddListener(TeardownClientMessageHandlers);
                    manager.onConnected.AddListener(OnConnectionStarted);
                    manager.onDisconnected.AddListener(OnConnectionEnded);
                    if ((manager.mode & NetworkManagerMode.ServerOnly) == NetworkManagerMode.ServerOnly) SetupServerMessageHandlers();
                    if ((manager.mode & NetworkManagerMode.ClientOnly) == NetworkManagerMode.ClientOnly) SetupClientMessageHandlers();
                }

                protected virtual void OnDestroy()
                {
                    manager.onServerStart.RemoveListener(SetupServerMessageHandlers);
                    manager.onClientStart.RemoveListener(SetupClientMessageHandlers);
                    manager.onServerStop.RemoveListener(TeardownServerMessageHandlers);
                    manager.onClientStop.RemoveListener(TeardownClientMessageHandlers);
                    manager.onConnected.RemoveListener(OnConnectionStarted);
                    manager.onDisconnected.RemoveListener(OnConnectionEnded);
                    if ((manager.mode & NetworkManagerMode.ServerOnly) == NetworkManagerMode.ServerOnly) TeardownServerMessageHandlers();
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

                /// <summary>
                ///   Sets the handlers for some to-server messages.
                ///     This method must be overriden to register
                ///     handlers for the subclasses of the abstract
                ///     generic messages.
                /// </summary>
                protected virtual void SetupServerMessageHandlers()
                {
                    // TODO a base implementation
                }

                /// <summary>
                ///   Sets the handlers for some to-client messages.
                ///     This method must be overriden to register
                ///     handlers for the subclasses of the abstract
                ///     generic messages.
                /// </summary>
                protected virtual void SetupClientMessageHandlers()
                {
                    // TODO a base implementation
                }

                /// <summary>
                ///   Clears the handlers for some to-server messages.
                /// </summary>
                protected virtual void TeardownServerMessageHandlers()
                {
                    // TODO a base implementation
                }

                /// <summary>
                ///   Clears the handlers for some to-client messages.
                /// </summary>
                protected virtual void TeardownClientMessageHandlers()
                {
                    // TODO a base implementation
                }

                /***********************************************************************************/
                /*********** Methods of account/character(s) data-fetching *************************/
                /*********** and also serialization to the client(s)       *************************/
                /***********************************************************************************/

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
                public abstract Messages.ChooseCharacter<CharacterID, CharacterPreviewData> MakeChooseCharacterMessage(IReadOnlyList<Tuple<CharacterID, CharacterPreviewData>> characters);

                /// <summary>
                ///   Builds a custom message to send the currently selected
                ///     character's full data.
                /// </summary>
                /// <param name="characterId">The id to serialize</param>
                /// <param name="characterFullData">The full data to serialize</param>
                /// <returns>The message to send</returns>
                public abstract Messages.UsingCharacter<CharacterID, CharacterFullData> MakeCurrentCharacterMessage(CharacterID characterId, CharacterFullData characterFullData);

                /// <summary>
                ///   Builds a custom message to send that a certain character id
                ///     is invalid.
                /// </summary>
                /// <param name="characterId">The id to serialize</param>
                /// <returns>The message to send</returns>
                public abstract Messages.InvalidCharacterID<CharacterID> MakeInvalidCharacterMessage(CharacterID characterId);

                /// <summary>
                ///   Builds a custom message to send that a certain character id
                ///     does not exist for a given account.
                /// </summary>
                /// <param name="characterId">The id to serialize</param>
                /// <returns>The message to send</returns>
                public abstract Messages.CharacterDoesNotExist<CharacterID> MakeNonExistingCharacterMessage(CharacterID characterId);

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
