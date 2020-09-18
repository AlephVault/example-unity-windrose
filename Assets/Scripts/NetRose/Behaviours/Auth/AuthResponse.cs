using System.Collections.Generic;
using Mirror;

namespace NetRose
{
    namespace Behaviours
    {
        namespace Auth
        {
            /// <summary>
            ///   An authentication response has 3 fields: whether the request was
            ///     successful, its code, and more details.
            /// </summary>
            public class AuthResponse : MessageBase
            {
                /// <summary>
                ///   Whether the request was successful (i.e. the user successfully
                ///     logged in).
                /// </summary>
                public bool IsSuccess = true;

                /// <summary>
                ///   A custom code, either for success or failure, for the status
                ///     of the authentication attempt.
                /// </summary>
                public string Code = "success";

                /// <summary>
                ///   More optional details regarding the result of the login attempt.
                /// </summary>
                public Dictionary<string, string> Details = new Dictionary<string, string>();

                /// <summary>
                ///   Serializes all the fields of this message. The details are serialized
                ///     as pairs of strings.
                /// </summary>
                /// <param name="writer">The writer to serialize this message into</param>
                public override void Serialize(NetworkWriter writer)
                {
                    writer.WriteBoolean(IsSuccess);
                    writer.WriteString(Code);
                    if (Details == null)
                    {
                        writer.WriteInt32(0);
                    }
                    else
                    {
                        writer.WriteInt32(Details.Count);
                        foreach (KeyValuePair<string, string> pair in Details)
                        {
                            writer.WriteString(pair.Key);
                            writer.WriteString(pair.Value);
                        }
                    }
                }

                /// <summary>
                ///   Deserializes all the fields of this message. The details are deserialized
                ///     from pairs of strings.
                /// </summary>
                /// <param name="reader">The reader to deserialize this message from</param>
                public override void Deserialize(NetworkReader reader)
                {
                    IsSuccess = reader.ReadBoolean();
                    Code = reader.ReadString();
                    int count = reader.ReadInt32();
                    Details = new Dictionary<string, string>();
                    for (int i = 0; i < count; i++)
                    {
                        string key = reader.ReadString();
                        string value = reader.ReadString();
                        Details[key] = value;
                    }
                }

                /// <summary>
                ///   Empty constructor for response objects.
                /// </summary>
                public AuthResponse() { }

                /// <summary>
                ///   Quick constructor for response objects.
                /// </summary>
                public AuthResponse(bool isSuccess, string code, Dictionary<string, string> details)
                {
                    IsSuccess = isSuccess;
                    Code = code;
                    Details = details == null ? new Dictionary<string, string>() : details;
                }
            }
        }
    }
}
