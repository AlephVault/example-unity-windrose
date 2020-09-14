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
                ///     data is not present as the authenticationData of the
                ///     connection. After sending this message, the connection
                ///     will be closed.
                /// </summary>
                public class MissingAccountID : IMessageBase
                {
                    public void Deserialize(NetworkReader reader) {}
                    public void Serialize(NetworkWriter writer) {}
                }
            }
        }
    }
}
