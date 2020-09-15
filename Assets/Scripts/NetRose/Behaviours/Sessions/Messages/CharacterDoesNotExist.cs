﻿using System;
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
                ///   This message is sent from server to client in multi-character
                ///     account games to tell that a chosen character does not exist
                ///     by its id.
                /// </summary>
                public abstract class CharacterDoesNotExist<CharacterID> : IMessageBase
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
