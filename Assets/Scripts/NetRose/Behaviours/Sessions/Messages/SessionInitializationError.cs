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
                ///   This message is sent by the server when the session
                ///     initialization runs into an unknown error.
                /// </summary>
                public class SessionInitializationError : IMessageBase
                {
                    public void Deserialize(NetworkReader reader) { }
                    public void Serialize(NetworkWriter writer) { }
                }
            }
        }
    }
}
