using System;
using Mirror;

namespace NetworkedSamples
{
    namespace Behaviours
    {
        namespace Sessions
        {
            public class SampleAuthMessage : MessageBase
            {
                public string Username;
                public string Password;

                public SampleAuthMessage() {}

                public SampleAuthMessage(string username, string password)
                {
                    Username = username;
                    Password = password;
                }

                public override void Deserialize(NetworkReader reader) {}
                public override void Serialize(NetworkWriter writer) {}
            }
        }
    }
}
