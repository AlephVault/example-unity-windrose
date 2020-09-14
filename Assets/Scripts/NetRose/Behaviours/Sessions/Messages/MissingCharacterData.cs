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
                ///   This message is sent by the server when an account
                ///     attempted to retrieve data using an invalid id
                ///     for the character (with respect to the account).
                /// </summary>
                public class MissingCharacterData : IMessageBase
                {
                    public void Deserialize(NetworkReader reader) { }
                    public void Serialize(NetworkWriter writer) { }
                }
            }
        }
    }
}
