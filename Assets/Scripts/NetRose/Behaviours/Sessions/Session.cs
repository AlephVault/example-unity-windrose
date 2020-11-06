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
            using Messages;

            /// <summary>
            ///   This is not just a session structure, but also notifies
            ///     all their changes to the listeners attached to it.
            /// </summary>
            public class Session<AID, AData, CharacterID, CharacterPreviewData, CharacterFullData, CCMsg, UCMsg, ICMsg, NCMsg>
                where CCMsg : ChooseCharacter<CharacterID, CharacterPreviewData>, new()
                where UCMsg : UsingCharacter<CharacterID, CharacterFullData>, new()
                where ICMsg : InvalidCharacterID<CharacterID>, new()
                where NCMsg : CharacterDoesNotExist<CharacterID>, new()
            {
                static readonly ILogger logger = LogFactory.GetLogger(typeof(Session<AID, AData, CharacterID, CharacterPreviewData, CharacterFullData, CCMsg, UCMsg, ICMsg, NCMsg>));

                /// <summary>
                ///   The manager this session is bound to.
                /// </summary>
                public readonly SessionManager<AID, AData, CharacterID, CharacterPreviewData, CharacterFullData, CCMsg, UCMsg, ICMsg, NCMsg> Manager;

                /// <summary>
                ///   The connection this session is related to.
                /// </summary>
                public readonly NetworkConnection Connection;

                // Keeps the session listeners.
                private HashSet<SessionListener<AID, AData, CharacterID, CharacterPreviewData, CharacterFullData, CCMsg, UCMsg, ICMsg, NCMsg>> listeners = new HashSet<SessionListener<AID, AData, CharacterID, CharacterPreviewData, CharacterFullData, CCMsg, UCMsg, ICMsg, NCMsg>>();

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
                    SessionManager<AID, AData, CharacterID, CharacterPreviewData, CharacterFullData, CCMsg, UCMsg, ICMsg, NCMsg> manager,
                    NetworkConnection connection, AID accountId
                ) {
                    Manager = manager;
                    Connection = connection;
                    AccountID = accountId;
                    AccountData = default(AData);
                    CurrentCharacterID = default(CharacterID);
                    CurrentCharacterData = default(CharacterFullData);
                    customData = new Dictionary<string, object>();
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
                        EachListener(delegate (SessionListener<AID, AData, CharacterID, CharacterPreviewData, CharacterFullData, CCMsg, UCMsg, ICMsg, NCMsg> listener) {
                            listener.CustomDataSet(key, value);
                        }, "custom-data-set");
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
                        EachListener(delegate (SessionListener<AID, AData, CharacterID, CharacterPreviewData, CharacterFullData, CCMsg, UCMsg, ICMsg, NCMsg> listener) {
                            listener.CustomDataRemoved(key);
                        }, "custom-data-removed");
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
                    EachListener(delegate (SessionListener<AID, AData, CharacterID, CharacterPreviewData, CharacterFullData, CCMsg, UCMsg, ICMsg, NCMsg> listener) {
                        listener.CustomDataCleared();
                    }, "custom-data-cleared");
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
                    catch(System.Exception e)
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
                        EachListener(delegate (SessionListener<AID, AData, CharacterID, CharacterPreviewData, CharacterFullData, CCMsg, UCMsg, ICMsg, NCMsg> listener) {
                            listener.AccountData();
                        }, "account-data");
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
                        CharacterFullData data = await Manager.GetCharacterData(AccountID, CurrentCharacterID);
                        if (!data.Equals(default(CharacterFullData)))
                        {
                            EachListener(delegate (SessionListener<AID, AData, CharacterID, CharacterPreviewData, CharacterFullData, CCMsg, UCMsg, ICMsg, NCMsg> listener) {
                                listener.UsingCharacter();
                            }, "using-character");
                        }
                    }
                    catch(System.Exception e)
                    {
                        AnErrorOccurred(string.Format("An exception was thrown while [re]loading the current character data for the session with account id: {0} and character id: {1}", AccountID, CurrentCharacterID), e);
                        return;
                    }
                }

                // Clears the current character, restarting the
                // appropriate flow to pick one.
                private async Task InitDefaultCharacterStatus()
                {
                    try
                    {
                        if (Manager.AccountsHaveMultipleCharacters())
                        {
                            if (IsUsingACharacter)
                            {
                                CurrentCharacterID = default(CharacterID);
                                CurrentCharacterData = default(CharacterFullData);
                                Connection.Send(new Messages.ReleasingCharacter());
                                EachListener(delegate (SessionListener<AID, AData, CharacterID, CharacterPreviewData, CharacterFullData, CCMsg, UCMsg, ICMsg, NCMsg> listener) {
                                    listener.UsingNoCharacter();
                                }, "using-no-character");
                                try
                                {
                                    CCMsg msg = new CCMsg();
                                    msg.Characters = await Manager.ListCharacters(AccountID);
                                    Connection.Send(msg);
                                }
                                catch (Exception e)
                                {
                                    AnErrorOccurred(string.Format("An exception was thrown while trying to load all the characters after releasing the current one for session with account id: {0}", AccountID), e);
                                }
                            }
                            else
                            {
                                CCMsg msg = new CCMsg();
                                msg.Characters = await Manager.ListCharacters(AccountID);
                                Connection.Send(msg);
                                EachListener(delegate (SessionListener<AID, AData, CharacterID, CharacterPreviewData, CharacterFullData, CCMsg, UCMsg, ICMsg, NCMsg> listener) {
                                    listener.UsingNoCharacter();
                                }, "using-no-character");
                            }
                        }
                        else
                        {
                            CharacterFullData data = await Manager.GetCharacterData(AccountID, default(CharacterID));
                            if (data.Equals(default(CharacterFullData)))
                            {
                                Connection.Send(new Messages.NoCharacterAvailable());
                                EachListener(delegate (SessionListener<AID, AData, CharacterID, CharacterPreviewData, CharacterFullData, CCMsg, UCMsg, ICMsg, NCMsg> listener) {
                                    listener.UsingNoCharacter();
                                }, "using-no-character");
                            }
                            else
                            {
                                CurrentCharacterID = default(CharacterID);
                                CurrentCharacterData = data;
                                UCMsg msg = new UCMsg();
                                msg.CurrentCharacterID = default(CharacterID);
                                msg.CurrentCharacterData = CurrentCharacterData;
                                Connection.Send(msg);
                                EachListener(delegate (SessionListener<AID, AData, CharacterID, CharacterPreviewData, CharacterFullData, CCMsg, UCMsg, ICMsg, NCMsg> listener) {
                                    listener.UsingCharacter();
                                }, "using-character");
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
                    bool multiple = Manager.AccountsHaveMultipleCharacters();
                    if (multiple)
                    {
                        if (CurrentCharacterID.Equals(default(CharacterID)))
                        {
                            ICMsg msg = new ICMsg();
                            msg.ID = characterId;
                            Connection.Send(msg);
                            Connection.Disconnect();
                            return;
                        }
                    }
                    else
                    {
                        if (!CurrentCharacterID.Equals(default(CharacterID)))
                        {
                            ICMsg msg = new ICMsg();
                            msg.ID = characterId;
                            Connection.Send(msg);
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
                        CharacterFullData data = await Manager.GetCharacterData(AccountID, characterId);
                        if (!data.Equals(default(CharacterFullData)))
                        {
                            CurrentCharacterID = characterId;
                            CurrentCharacterData = data;
                            UCMsg msg = new UCMsg();
                            msg.CurrentCharacterID = characterId;
                            msg.CurrentCharacterData = CurrentCharacterData;
                            Connection.Send(msg);
                            EachListener(delegate (SessionListener<AID, AData, CharacterID, CharacterPreviewData, CharacterFullData, CCMsg, UCMsg, ICMsg, NCMsg> listener) {
                                listener.UsingCharacter();
                            }, "using-character");
                        }
                        else if (multiple)
                        {
                            NCMsg msg = new NCMsg();
                            msg.ID = characterId;
                            Connection.Send(msg);
                        }
                        else
                        {
                            Connection.Send(new Messages.NoCharacterAvailable());
                            EachListener(delegate (SessionListener<AID, AData, CharacterID, CharacterPreviewData, CharacterFullData, CCMsg, UCMsg, ICMsg, NCMsg> listener) {
                                listener.UsingNoCharacter();
                            }, "using-no-character");
                        }
                    }
                    catch (Exception e)
                    {
                        AnErrorOccurred(string.Format("An exception was thrown while [re]loading the current character data for the session with account id: {0} and character id: {1}", AccountID, CurrentCharacterID), e);
                    }
                }

                /// <summary>
                ///   Releases the currently selected character. A character
                ///     must currently be selected, and this method can only
                ///     be invoked in multi-character account games.
                /// </summary>
                /// <returns></returns>
                public async void ReleaseCharacter()
                {
                    if (!Manager.AccountsHaveMultipleCharacters())
                    {
                        Connection.Send(new Messages.CannotReleaseCharacterInSingleMode());
                        return;
                    }

                    if (CurrentCharacterData.Equals(default(CharacterFullData)))
                    {
                        Connection.Send(new Messages.NotUsingCharacter());
                        return;
                    }

                    CurrentCharacterID = default(CharacterID);
                    CurrentCharacterData = default(CharacterFullData);
                    Connection.Send(new Messages.ReleasingCharacter());
                    EachListener(delegate (SessionListener<AID, AData, CharacterID, CharacterPreviewData, CharacterFullData, CCMsg, UCMsg, ICMsg, NCMsg> listener) {
                        listener.UsingNoCharacter();
                    }, "using-no-character");
                    try
                    {
                        List<Tuple<CharacterID, CharacterPreviewData>> characters = await Manager.ListCharacters(AccountID);
                        CCMsg msg = new CCMsg();
                        msg.Characters = characters;
                        Connection.Send(msg);
                    }
                    catch (Exception e)
                    {
                        AnErrorOccurred(string.Format("An exception was thrown while trying to load all the characters after releasing the current one for session with account id: {0}", AccountID), e);
                    }
                }

                /// <summary>
                ///   Resets the session to its default state. This implies:
                ///     - [Re]Loads the account data.
                ///     - Starts a flow for the multi/single character(s).
                ///     - Clears all the user data.
                /// </summary>
                public async Task Reset()
                {
                    await RefreshAccountData();
                    await InitDefaultCharacterStatus();
                    Clear();
                }

                /// <summary>
                ///   Adds a listener to this session, and notifies.
                /// </summary>
                /// <param name="listener">The listener to add</param>
                public void AddListener(SessionListener<AID, AData, CharacterID, CharacterPreviewData, CharacterFullData, CCMsg, UCMsg, ICMsg, NCMsg> listener)
                {
                    if (!listeners.Contains(listener))
                    {
                        listeners.Add(listener);
                        try
                        {
                            listener.Started(this);
                        }
                        catch (System.Exception e)
                        {
                            logger.LogError("Exception while forwarding an event: started");
                            logger.LogException(e);
                        }
                    }
                }

                /// <summary>
                ///   Removes a listener from this session, and notifies.
                /// </summary>
                /// <param name="listener">The listener to add</param>
                public void RemoveListener(SessionListener<AID, AData, CharacterID, CharacterPreviewData, CharacterFullData, CCMsg, UCMsg, ICMsg, NCMsg> listener)
                {
                    if (listeners.Contains(listener))
                    {
                        try
                        {
                            listener.Terminated();
                        }
                        catch (System.Exception e)
                        {
                            logger.LogError("Exception while forwarding an event: terminated");
                            logger.LogException(e);
                        }
                        listeners.Remove(listener);
                    }
                }

                /// <summary>
                ///   Clears all the listenrs from this session, and notifies.
                /// </summary>
                public void ClearListeners()
                {
                    foreach(SessionListener<AID, AData, CharacterID, CharacterPreviewData, CharacterFullData, CCMsg, UCMsg, ICMsg, NCMsg> listener in listeners)
                    {
                        try
                        {
                            listener.Terminated();
                        }
                        catch (System.Exception e)
                        {
                            logger.LogError("Exception while forwarding an event: terminated");
                            logger.LogException(e);
                        }
                    }
                    listeners.Clear();
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
                        return IsActive && !CurrentCharacterData.Equals(default(CharacterFullData));
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
                private void AnErrorOccurred(string message, System.Exception e)
                {
                    logger.LogError(message);
                    logger.LogException(e);
                    Connection.Send(new Messages.SessionUnknownError());
                    Connection.Disconnect();
                }

                // Runs a single event.
                private void EachListener(Action<SessionListener<AID, AData, CharacterID, CharacterPreviewData, CharacterFullData, CCMsg, UCMsg, ICMsg, NCMsg>> action, string key)
                {
                    foreach (SessionListener<AID, AData, CharacterID, CharacterPreviewData, CharacterFullData, CCMsg, UCMsg, ICMsg, NCMsg> listener in listeners)
                    {
                        try
                        {
                            action(listener);
                        }
                        catch(System.Exception e)
                        {
                            logger.LogError("Exception while forwarding an event: " + key);
                            logger.LogException(e);
                        }
                    }
                }
            }
        }
    }
}
