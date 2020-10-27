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
                ///   This message is sent by the server to the client
                ///     when successfully releasing the current in-use
                ///     character.
                /// </summary>
                public class ReleasingCharacter : IMessageBase
                {
                    public void Deserialize(NetworkReader reader) { }
                    public void Serialize(NetworkWriter writer) { }
                }
            }
        }
    }
}
