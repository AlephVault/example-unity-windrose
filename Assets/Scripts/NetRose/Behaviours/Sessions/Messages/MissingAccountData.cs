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
                ///   This message is sent by the server when the authentication
                ///     data is present but there is no such account (due to a
                ///     race condition between authentication and retrieval).
                /// </summary>
                public class MissingAccountData : IMessageBase
                {
                    public void Deserialize(NetworkReader reader) { }
                    public void Serialize(NetworkWriter writer) { }
                }
            }
        }
    }
}
