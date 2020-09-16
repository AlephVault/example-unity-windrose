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
            ///   A session listener implements all the events involving data changes
            ///     in the session (regarding account, character, and custom data).
            /// </summary>
            public interface SessionListener<AID, AData, CharacterID, CharacterPreviewData, CharacterFullData>
            {
                /// <summary>
                ///   Called when this listener is connected to the session.
                /// </summary>
                void Started(Session<AID, AData, CharacterID, CharacterPreviewData, CharacterFullData> session);

                /// <summary>
                ///   Called when a value in the custom data has changed.
                /// </summary>
                /// <param name="key">The involved key</param>
                /// <param name="value">The new value</param>
                void CustomDataSet(string key, object value);

                /// <summary>
                ///   Called to notify that the current session's account
                ///     data is being refreshed.
                /// </summary>
                void AccountData();

                /// <summary>
                ///   Called to notify that the current session is using
                ///     a character (the session can be directly queried
                ///     to get the character id/data).
                /// </summary>
                void UsingCharacter();

                /// <summary>
                ///   Called to notify that the current session is not using
                ///     a character.
                /// </summary>
                void UsingNoCharacter();

                /// <summary>
                ///   Called when a value in the custom data has been removed.
                /// </summary>
                /// <param name="key">The involved key</param>
                void CustomDataRemoved(string key);

                /// <summary>
                ///   Called when all the custom data has been cleared.
                /// </summary>
                void CustomDataCleared();

                /// <summary>
                ///   Called when the session is terminated (via disconnection).
                /// </summary>
                void Terminated();
            }
        }
    }
}
