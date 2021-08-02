using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;
using MLAPI.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace AlephVault.Unity.MMO.Samples
{
    namespace Behaviours
    {
        namespace Realms
        {
            [RequireComponent(typeof(NetworkObject))]
            public class ChatRoom : NetworkBehaviour
            {
                // All of the available channels, by their names.
                private static Dictionary<string, ChatRoom> allChannels = new Dictionary<string, ChatRoom>();

                /// <summary>
                ///   A chat message has the sender (as a string),
                ///   a stringified send date, and the content of
                ///   the message.
                /// </summary>
                public class ChatMessage : INetworkSerializable
                {
                    public string UserName;
                    public string Date;
                    public string Body;

                    public void NetworkSerialize(NetworkSerializer serializer)
                    {
                        serializer.Serialize(ref UserName);
                        serializer.Serialize(ref Date);
                        serializer.Serialize(ref Body);
                    }
                }

                /// <summary>
                ///   The room name. Only set by the server.
                /// </summary>
                public readonly NetworkVariable<string> RoomName = new NetworkVariable<string>(new NetworkVariableSettings()
                {
                    ReadPermission = NetworkVariablePermission.Everyone,
                    WritePermission = NetworkVariablePermission.ServerOnly
                });

                /// <summary>
                ///   Contains the list of users in the room.
                /// </summary>
                public readonly NetworkList<ChatUser> Users = new NetworkList<ChatUser>(new NetworkVariableSettings()
                {
                    ReadPermission = NetworkVariablePermission.Everyone,
                    WritePermission = NetworkVariablePermission.ServerOnly
                });

                /// <summary>
                ///   Contains the list of chat messages in the room.
                /// </summary>
                public readonly NetworkList<ChatMessage> Messages = new NetworkList<ChatMessage>(new NetworkVariableSettings()
                {
                    ReadPermission = NetworkVariablePermission.Everyone,
                    WritePermission = NetworkVariablePermission.ServerOnly
                });

                /// <summary>
                ///   Finds a channel by its name, or returns null of not found.
                /// </summary>
                /// <param name="name">The name to find a channel by</param>
                /// <returns>The result - either a channel, or null.</returns>
                public static ChatRoom Find(string name)
                {
                    ChatRoom result;
                    allChannels.TryGetValue(name, out result);
                    return result;
                }

                private void Start()
                {
                    allChannels.Add(RoomName.Value, this);
                }

                private void OnDestroy()
                {
                    Users.Clear();
                    allChannels.Remove(RoomName.Value);
                }
            }
        }
    }
}