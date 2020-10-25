using System;
using System.Collections.Generic;
using System.Linq;
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
                    ///   Reads a character id from the reader.
                    /// </summary>
                    /// <param name="reader">The reader to read the character id from</param>
                    /// <returns>The read character id</returns>
                    protected abstract CharacterID ReadCharacterID(NetworkReader reader);

                    /// <summary>
                    ///   Reads a character preview data from the reader.
                    /// </summary>
                    /// <param name="reader">The reader to read the character preview data from</param>
                    /// <returns>The read character preview data</returns>
                    protected abstract CharacterPreviewData ReadCharacterPreviewData(NetworkReader reader);

                    /// <summary>
                    ///   Writes a character id into the writer.
                    /// </summary>
                    /// <param name="writer">The writer to write the character id into</param>
                    /// <param name="id">The character id to write</param>
                    protected abstract void WriteCharacterID(NetworkWriter writer, CharacterID id);

                    /// <summary>
                    ///   Writes a character preview data into the writer.
                    /// </summary>
                    /// <param name="reader">The writer to write the character preview data into</param>
                    /// <param name="data">The character data to write</param>
                    protected abstract void WriteCharacterPreviewData(NetworkWriter writer, CharacterPreviewData data);

                    /// <summary>
                    ///   Appropriately populates the message from the reader.
                    /// </summary>
                    /// <param name="reader">The reader to populate from</param>
                    public void Deserialize(NetworkReader reader)
                    {
                        List<Tuple<CharacterID, CharacterPreviewData>> characters = new List<Tuple<CharacterID, CharacterPreviewData>>();
                        int count = reader.ReadInt32();
                        for (int i = 0; i < count; i++) {
                            CharacterID key = ReadCharacterID(reader);
                            CharacterPreviewData value = ReadCharacterPreviewData(reader);
                            characters.Append(new Tuple<CharacterID, CharacterPreviewData>(key, value));
                        }
                        Characters = characters;
                    }

                    /// <summary>
                    ///   Appropriately dumps the message into the writer.
                    /// </summary>
                    /// <param name="writer">The reader to dump the data into</param>
                    public void Serialize(NetworkWriter writer)
                    {
                        if (Characters == null)
                        {
                            writer.WriteInt32(0);
                        }
                        else
                        {
                            writer.WriteInt32(Characters.Count);
                            foreach (Tuple<CharacterID, CharacterPreviewData> pair in Characters)
                            {
                                WriteCharacterID(writer, pair.Item1);
                                WriteCharacterPreviewData(writer, pair.Item2);
                            }
                        }
                    }
                }
            }
        }
    }
}
