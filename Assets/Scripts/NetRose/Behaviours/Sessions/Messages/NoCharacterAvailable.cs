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
                ///     when the only character (this is for the single
                ///     character account games) has not yet been created
                ///     or is somehow not available.
                /// </summary>
                public class NoCharacterAvailable : IMessageBase
                {
                    public void Deserialize(NetworkReader reader) {}
                    public void Serialize(NetworkWriter writer) {}
                }
            }
        }
    }
}
