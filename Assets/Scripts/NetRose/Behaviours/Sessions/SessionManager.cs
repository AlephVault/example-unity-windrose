using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

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

                /// <summary>
                ///   What to do if an account is logging again (i.e. how to handle
                ///     multiple temporary simultaneous copies of the same accounts).
                /// </summary>
                [SerializeField]
                DuplicateAccountRule ResolveDuplicates = DuplicateAccountRule.Kick;

                private void Awake()
                {
                    manager = (NetworkManager.singleton as NetworkWorldManager);
                    if (manager == null)
                    {
                        Destroy(gameObject);
                        throw new Exception("The NetworkManager singleton must be of type NetworkWorldManager");
                    }
                    manager.onConnected.AddListener(OnConnectionStarted);
                    manager.onDisconnected.AddListener(OnConnectionEnded);
                }

                private void OnDestroy()
                {
                    manager.onConnected.RemoveListener(OnConnectionStarted);
                    manager.onDisconnected.RemoveListener(OnConnectionEnded);
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
                        // To this point, the player does not exist.
                        // Just initialize the sessions without calling
                        // any callback.
                        AccountID accountId = (AccountID)connection.authenticationData;
                        ClearSession(accountId);
                    }
                }

                /***********************************************************************************/
                /*********** Methods of account/character(s) data-fetching *************************/
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

                /***********************************************************************************/
                /***********************************************************************************/
                /***********************************************************************************/

                private async void InitSession(NetworkConnection connection, AccountID accountId)
                {
                    AccountData data;
                    try
                    {
                        data = await GetAccountData(accountId);
                    }
                    catch(Exception e)
                    {
                        // This is a kind of diaper: all the exceptions will be captured,
                        // logged, and blind-notified to the client.
                        logger.LogError("An exception was thrown while initializing a new session");
                        logger.LogException(e);
                        connection.Send(new Messages.SessionInitializationError());
                        return;
                    }

                    if (data.Equals(default(AccountData)))
                    {
                        connection.Send(new Messages.MissingAccountData());
                        connection.Disconnect();
                    }
                    else
                    {
                        // If there is a connection with the same account id, it must
                        // kick either of the connections.
                        NetworkConnection otherConnection;
                        if (connectionByAccountID.TryGetValue(accountId, out otherConnection))
                        {
                            if (ResolveDuplicates == DuplicateAccountRule.Kick)
                            {
                                connection.Send(new Messages.DupeKicked());
                                connection.Disconnect();
                                // Nothing else occurs here to this connection.
                            }
                            else // DuplicateAccountRule.Ghost
                            {
                                connection.Send(new Messages.DupeGhosted());
                                otherConnection.Disconnect();
                                // TODO:
                                // - PUT session data.
                                // - Network-Notify session started.
                                // - Start session flow (e.g. "empty" or "pick character").
                                //   - Notify session started (callbacks).
                            }
                        }
                        else
                        {
                            // TODO:
                            // - PUT session data.
                            // - Network-Notify session started.
                            // - Start session flow (e.g. "empty" or "pick character").
                            //   - Notify session stopped (callbacks).
                        }
                    }
                }

                /***************************************************************************/

                // Clearing the session is only needed INSIDE this class.
                private void ClearSession(AccountID accountId)
                {
                    // TODO:
                    // - CLEAR session data, if any.
                    //   - Notify session clear (callbacks, behaviours).
                }
            }
        }
    }
}
