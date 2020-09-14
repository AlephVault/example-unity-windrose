using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            public class Session<AID, AData, CharacterID, CharacterFullData>
            {
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
                public Session(NetworkConnection connection, AID accountId, AData accountData)
                {
                    Connection = connection;
                    AccountID = accountId;
                    AccountData = accountData;
                    CurrentCharacterID = default(CharacterID);
                    CurrentCharacterData = default(CharacterFullData);
                    customData = new Dictionary<string, object>();
                }

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

                public void Clear()
                {
                    customData.Clear();
                    // TODO event: custom data cleared.
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
                        // Two distinct cases are covered here: the current
                        //   character ID is not the default one (for the
                        //   multi-character mode) or (for the single-character
                        //   account mode which uses the default value for
                        //   character id) the character full data is not the
                        //   default value.
                        return !CurrentCharacterID.Equals(default(CharacterID)) || !CurrentCharacterData.Equals(default(CharacterFullData));
                    }
                }
            }
        }
    }
}
