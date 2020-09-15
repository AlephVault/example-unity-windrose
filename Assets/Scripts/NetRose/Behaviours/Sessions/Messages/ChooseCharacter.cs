using System;
using System.Collections.Generic;
using Mirror;

namespace NetRose
{
    namespace Behaviours
    {
        namespace Sessions
        {
            namespace Messages
            {
                /// <summary>
                ///   This is an abstract message and must be implemented
                ///     to properly serialize the preview list.
                /// </summary>
                public abstract class ChooseCharacter<CharacterID, CharacterPreviewData> : IMessageBase
                {
                    /// <summary>
                    ///   The character list to send to the client.
                    /// </summary>
                    public IReadOnlyList<Tuple<CharacterID, CharacterPreviewData>> Characters { get; protected set; }

                    /// <summary>
                    ///   Implement this to correctly populate the characters from a reader.
                    /// </summary>
                    /// <param name="reader">The reader to populate from</param>
                    public abstract void Deserialize(NetworkReader reader);

                    /// <summary>
                    ///   Implement this to correctly dump the characters into the reader.
                    /// </summary>
                    /// <param name="writer">The reader to dump the data into</param>
                    public abstract void Serialize(NetworkWriter writer);
                }
            }
        }
    }
}
