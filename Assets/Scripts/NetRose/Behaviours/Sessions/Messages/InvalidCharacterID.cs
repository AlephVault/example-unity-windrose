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
                ///     to properly serialize the character id.
                /// </summary>
                public abstract class InvalidCharacterID<CharacterID> : IMessageBase
                {
                    /// <summary>
                    ///   The character id to send to the client.
                    /// </summary>
                    public CharacterID ID { get; protected set; }

                    /// <summary>
                    ///   Implement this to correctly populate the character id from a reader.
                    /// </summary>
                    /// <param name="reader">The reader to populate from</param>
                    public abstract void Deserialize(NetworkReader reader);

                    /// <summary>
                    ///   Implement this to correctly dump the character id into the reader.
                    /// </summary>
                    /// <param name="writer">The reader to dump the data into</param>
                    public abstract void Serialize(NetworkWriter writer);
                }
            }
        }
    }
}
