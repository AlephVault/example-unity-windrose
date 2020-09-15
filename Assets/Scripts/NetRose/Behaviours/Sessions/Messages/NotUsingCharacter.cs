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
                ///   This message is sent from the server to tell that
                ///     the client is not using a character.
                /// </summary>
                public class NotUsingCharacter : IMessageBase
                {
                    /// <summary>
                    ///   Implement this to correctly populate the character id from a reader.
                    /// </summary>
                    /// <param name="reader">The reader to populate from</param>
                    public void Deserialize(NetworkReader reader) { }

                    /// <summary>
                    ///   Implement this to correctly dump the character id into the reader.
                    /// </summary>
                    /// <param name="writer">The reader to dump the data into</param>
                    public void Serialize(NetworkWriter writer) { }
                }
            }
        }
    }
}
