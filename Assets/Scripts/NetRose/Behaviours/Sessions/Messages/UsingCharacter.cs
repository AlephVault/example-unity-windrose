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
                    public CharacterFullData CurrentCharacterData { get; set; }

                    /// <summary>
                    ///   The id of the currently in-use character.
                    /// </summary>
                    public CharacterID CurrentCharacterID { get; set; }

                    /// <summary>
                    ///   Reads a character id from the reader.
                    /// </summary>
                    /// <param name="reader">The reader to read the character id from</param>
                    /// <returns>The read character id</returns>
                    protected abstract CharacterID ReadCharacterID(NetworkReader reader);

                    /// <summary>
                    ///   Reads a character full data from the reader.
                    /// </summary>
                    /// <param name="reader">The reader to read the character full data from</param>
                    /// <returns>The read character full data</returns>
                    protected abstract CharacterFullData ReadCharacterFullData(NetworkReader reader);

                    /// <summary>
                    ///   Writes a character id into the writer.
                    /// </summary>
                    /// <param name="writer">The writer to write the character id into</param>
                    /// <param name="id">The character id to write</param>
                    protected abstract void WriteCharacterID(NetworkWriter writer, CharacterID id);

                    /// <summary>
                    ///   Writes a character full data into the writer.
                    /// </summary>
                    /// <param name="reader">The writer to write the character preview data into</param>
                    /// <param name="data">The character full data to write</param>
                    protected abstract void WriteCharacterFullData(NetworkWriter writer, CharacterFullData data);

                    /// <summary>
                    ///   Appropriately reads the character id and full data from the reader.
                    /// </summary>
                    /// <param name="reader">The reader to populate from</param>
                    public void Deserialize(NetworkReader reader)
                    {
                        CurrentCharacterID = ReadCharacterID(reader);
                        CurrentCharacterData = ReadCharacterFullData(reader);
                    }

                    /// <summary>
                    ///   Appropriately dumps the character id and full data into the reader.
                    /// </summary>
                    /// <param name="writer">The reader to dump the data into</param>
                    public void Serialize(NetworkWriter writer)
                    {
                        WriteCharacterID(writer, CurrentCharacterID);
                        WriteCharacterFullData(writer, CurrentCharacterData);
                    }
                }
            }
        }
    }
}
