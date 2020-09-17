using System;
using Mirror;

namespace NetworkedSamples
{
    namespace Behaviours
    {
        namespace Sessions
        {
            public class SampleAuthMessage : IMessageBase
            {
                public string Username;
                public string Password;

                public SampleAuthMessage() {}

                public SampleAuthMessage(string username, string password)
                {
                    Username = username;
                    Password = password;
                }

                public void Deserialize(NetworkReader reader) {}
                public void Serialize(NetworkWriter writer) {}
            }
        }
    }
}
