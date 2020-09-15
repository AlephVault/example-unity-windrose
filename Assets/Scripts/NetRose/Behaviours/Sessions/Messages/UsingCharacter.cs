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
                ///     to properly serialize the full data of the current
                ///     character being used.
                /// </summary>
                public abstract class UsingCharacter<CharacterID, CharacterFullData> : IMessageBase
                {
                    /// <summary>
                    ///   The full data of the currently in-use character.
                    /// </summary>
                    public CharacterFullData CurrentCharacterData { get; protected set; }

                    /// <summary>
                    ///   The id of the currently in-use character.
                    /// </summary>
                    public CharacterID CurrentCharacterID { get; protected set; }

                    /// <summary>
                    ///   Implement this to correctly populate the character from a reader.
                    /// </summary>
                    /// <param name="reader">The reader to populate from</param>
                    public abstract void Deserialize(NetworkReader reader);

                    /// <summary>
                    ///   Implement this to correctly dump the character into the reader.
                    /// </summary>
                    /// <param name="writer">The reader to dump the data into</param>
                    public abstract void Serialize(NetworkWriter writer);
                }
            }
        }
    }
}
