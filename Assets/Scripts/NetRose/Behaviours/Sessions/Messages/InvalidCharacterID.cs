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
                    public CharacterID ID { get; set; }

                    /// <summary>
                    ///   Reads a character id from the reader.
                    /// </summary>
                    /// <param name="reader">The reader to read the character id from</param>
                    /// <returns>The read character id</returns>
                    protected abstract CharacterID ReadCharacterID(NetworkReader reader);

                    /// <summary>
                    ///   Writes a character id into the writer.
                    /// </summary>
                    /// <param name="writer">The writer to write the character id into</param>
                    /// <param name="id">The character id to write</param>
                    protected abstract void WriteCharacterID(NetworkWriter writer, CharacterID id);

                    /// <summary>
                    ///   Populates the non-existing character id from the reader.
                    /// </summary>
                    /// <param name="reader">The reader to populate from</param>
                    public void Deserialize(NetworkReader reader)
                    {
                        ID = ReadCharacterID(reader);
                    }

                    /// <summary>
                    ///   Implement this to correctly dump the character id into the reader.
                    /// </summary>
                    /// <param name="writer">The reader to dump the data into</param>
                    public void Serialize(NetworkWriter writer)
                    {
                        WriteCharacterID(writer, ID);
                    }
                }
            }
        }
    }
}
