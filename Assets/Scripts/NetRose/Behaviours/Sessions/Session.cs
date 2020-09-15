using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Mirror;

namespace NetRose
{
    namespace Behaviours
    {
        namespace Sessions
        {
            /// <summary>
            ///   This is not just a session structure, but also notifies
            ///     all their changes to the listeners attached to it.
            /// </summary>
            public class Session<AID, AData, CharacterID, CharacterPreviewData, CharacterFullData>
            {
                static readonly ILogger logger = LogFactory.GetLogger(typeof(Session<AID, AData, CharacterID, CharacterPreviewData, CharacterFullData>));

                /// <summary>
                ///   The manager this session is bound to.
                /// </summary>
                public readonly SessionManager<AID, AData, CharacterID, CharacterPreviewData, CharacterFullData> Manager;

                /// <summary>
                ///   The connection this session is related to.
                /// </summary>
                public readonly NetworkConnection Connection;

                /// <summary>
                ///   The id of the account in the current session.
                /// </summary>
                public readonly AID AccountID;

                /// <summary>
                ///   The main data of the account. It is retrieved by the
                ///     manager when the session is created, and can be
                ///     updated later.
                /// </summary>
                public AData AccountData { get; private set; }

                /// <summary>
                ///   The id of the current character. It will be
                ///     <c>default(CharacterID)</c> if no character
                ///     is selected in multi-character accounts,
                ///     but it will always be such value is the
                ///     account system uses single-character per
                ///     account.
                /// </summary>
                public CharacterID CurrentCharacterID { get; private set; }

                /// <summary>
                ///   The data of the current character. It will
                ///     be <c>default(CharacterFullData)</c> if
                ///     a character is selected.
                /// </summary>
                public CharacterFullData CurrentCharacterData { get; private set; }

                // Per-session custom data.
                private Dictionary<string, object> customData = new Dictionary<string, object>();

                /// <summary>
                ///   The session is initialized with some data and will
                ///     not trigger any event on its own (no object is
                ///     related to the current connection... yet).
                /// </summary>
                /// <param name="connection">The connection supporting the session</param>
                /// <param name="accountId">The account id linked to the session</param>
                /// <param name="accountData">The current data of the account</param>
                public Session(
                    SessionManager<AID, AData, CharacterID, CharacterPreviewData, CharacterFullData> manager,
                    NetworkConnection connection, AID accountId
                ) {
                    Manager = manager;
                    Connection = connection;
                    AccountID = accountId;
                    AccountData = default(AData);
                    CurrentCharacterID = default(CharacterID);
                    CurrentCharacterData = default(CharacterFullData);
                    customData = new Dictionary<string, object>();
                    Connection.Send(new Messages.SessionStarted());
                }

                /// <summary>
                ///   Sets and retrieves custom session data.
                /// </summary>
                /// <param name="key">The key to set/retrieve the data for</param>
                /// <returns>The ression data for that key</returns>
                public object this[string key]
                {
                    get
                    {
                        return customData[key];
                    }
                    set
                    {
                        customData[key] = value;
                        // TODO event: custom data set (key, value).
                    }
                }

                /// <summary>
                ///   Removes a single field from the custom /preferences
                ///     session data.
                /// </summary>
                /// <param name="key">The key to remove</param>
                /// <returns>Whether it was removed or not</returns>
                public bool Remove(string key)
                {
                    if (customData.Remove(key))
                    {
                        // TODO event: custom data removed (key).
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                /// <summary>
                ///   Clears all the custom / preferences data from
                ///     the session.
                /// </summary>
                public void Clear()
                {
                    customData.Clear();
                    // TODO event: custom data cleared.
                }

                /// <summary>
                ///   Reloads the account data for this session. This
                ///     task is asynchronous and must be waited for.
                /// </summary>
                public async Task RefreshAccountData()
                {
                    try
                    {
                        AccountData = await Manager.GetAccountData(AccountID);
                    }
                    catch(Exception e)
                    {
                        AnErrorOccurred(string.Format("An exception was thrown while [re]loading the account data for the session with account id: {0}", AccountID), e);
                        return;
                    }

                    if (AccountData.Equals(default(AData)))
                    {
                        logger.LogFormat(LogType.Log, "Missing account data for account id: {0}", AccountID);
                        Connection.Send(new Messages.MissingAccountData());
                        Connection.Disconnect();
                    }
                    else
                    {
                        // TODO event: account data refreshed.
                    }
                }

                /// <summary>
                ///   Reloads the character data for this session. This
                ///     task is asynchronous and must be waited for.
                /// </summary>
                public async Task RefreshCharacterData()
                {
                    try
                    {
                        CurrentCharacterData = await Manager.GetCharacterData(AccountID, CurrentCharacterID);
                        // TODO event: character data refreshed.
                    }
                    catch(Exception e)
                    {
                        AnErrorOccurred(string.Format("An exception was thrown while [re]loading the current character data for the session with account id: {0} and character id: {1}", AccountID, CurrentCharacterID), e);
                        return;
                    }
                }

                // Clears the current character, restarting the
                // appropriate flow to pick one.
                private async Task InitDefaultCharacterStatus()
                {
                    // TODO fix this method - it is considered to work the first time
                    // TODO but it is allowed to be called multiple times. So the logic
                    // TODO must be changed as well.
                    try
                    {
                        if (Manager.AccountsHaveMultipleCharacters())
                        {
                            Connection.Send(Manager.MakeChooseCharacterMessage(await Manager.ListCharacters(AccountID)));
                            // TODO event: no character selected.
                        }
                        else
                        {
                            CharacterFullData data = await Manager.GetCharacterData(AccountID, default(CharacterID));
                            if (data.Equals(default(CharacterFullData)))
                            {
                                Connection.Send(new Messages.NoCharacterAvailable());
                                // TODO event: no character available.
                            }
                            else
                            {
                                CurrentCharacterID = default(CharacterID);
                                CurrentCharacterData = data;
                                Connection.Send(Manager.MakeCurrentCharacterMessage(CurrentCharacterID, CurrentCharacterData));
                                // TODO event: character selected.
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        AnErrorOccurred(string.Format("An exception was thrown while [re]starting the whole session flow for a session with account id: {0}", AccountID), e);
                    }
                }

                /// <summary>
                ///   Picks a character from the available ones. The character
                ///     id must be valid, and this method can only be invoked
                ///     while not already having a character. The id of the
                ///     character must be the default in single-character
                ///     account games, and not the default in multi-character
                ///     account games. Failure to meet the conditions, or errors
                ///     being raised, will result in the session being terminated.
                ///     This task is asynchronous and must be waited for.
                /// </summary>
                /// <param name="characterId">The ID of the character to pick</param>
                public async Task PickCharacter(CharacterID characterId)
                {
                    if (Manager.AccountsHaveMultipleCharacters())
                    {
                        if (CurrentCharacterID.Equals(default(CharacterID)))
                        {
                            Connection.Send(Manager.MakeInvalidCharacterMessage(characterId));
                            Connection.Disconnect();
                            return;
                        }
                    }
                    else
                    {
                        if (!CurrentCharacterID.Equals(default(CharacterID)))
                        {
                            Connection.Send(Manager.MakeInvalidCharacterMessage(characterId));
                            Connection.Disconnect();
                            return;
                        }
                    }

                    if (!CurrentCharacterData.Equals(default(CharacterFullData)))
                    {
                        Connection.Send(new Messages.AlreadyUsingCharacter());
                        Connection.Disconnect();
                        return;
                    }

                    try
                    {
                        CurrentCharacterData = await Manager.GetCharacterData(AccountID, characterId);
                        // TODO event: character selected.
                        // TODO message: character selected.
                    }
                    catch (Exception e)
                    {
                        AnErrorOccurred(string.Format("An exception was thrown while [re]loading the current character data for the session with account id: {0} and character id: {1}", AccountID, CurrentCharacterID), e);
                        return;
                    }
                }

                /// <summary>
                ///   Releases the currently selected character. A character
                ///     must currently be selected, and this method can only
                ///     be invoked in multi-character account games.
                /// </summary>
                /// <returns></returns>
                public void ReleaseCharacter()
                {
                    if (!Manager.AccountsHaveMultipleCharacters())
                    {
                        // TODO message: cannot release a character in single-character games.
                        Connection.Disconnect();
                        return;
                    }

                    if (CurrentCharacterData.Equals(default(CharacterFullData)))
                    {
                        // TODO message: cannot release a character while none is picked.
                        Connection.Disconnect();
                        return;
                    }

                    CurrentCharacterID = default(CharacterID);
                    CurrentCharacterData = default(CharacterFullData);
                    // TODO message: character released.
                    // TODO event: character released.
                    // TODO reload character list.
                    // TODO message: please select character.
                }

                /// <summary>
                ///   Resets the session to its default state. This implies:
                ///     - [Re]Loads the account data.
                ///     - Starts a flow for the multi/single character(s).
                ///     - Clears all the user data.
                /// </summary>
                /// <returns></returns>
                public async Task Reset()
                {
                    await RefreshAccountData();
                    await InitDefaultCharacterStatus();
                    Clear();
                }

                /// <summary>
                ///   Tells whether this sessions is currently
                ///     using a character or is in the selection
                ///     mode (or creation mode).
                /// </summary>
                public bool IsUsingACharacter
                {
                    get
                    {
                        // Checking that the character full data is not empty
                        // should always be enough.
                        return !CurrentCharacterData.Equals(default(CharacterFullData));
                    }
                }

                /// <summary>
                ///   Tells whether this session is active or was terminated.
                /// </summary>
                public bool IsActive
                {
                    get
                    {
                        return Manager.HasSession(this);
                    }
                }

                // Logs the error, blindly notifies to the client, and ends.
                private void AnErrorOccurred(string message, Exception e)
                {
                    logger.LogError(message);
                    logger.LogException(e);
                    Connection.Send(new Messages.SessionUnknownError());
                    Connection.Disconnect();
                }
            }
        }
    }
}
