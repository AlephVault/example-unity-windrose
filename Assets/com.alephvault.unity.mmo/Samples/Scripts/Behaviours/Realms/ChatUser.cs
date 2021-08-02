using AlephVault.Unity.MMO.Types;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlephVault.Unity.MMO.Samples
{
    namespace Behaviours
    {
        namespace Realms
        {
            [RequireComponent(typeof(NetworkObject))]
            public class ChatUser : NetworkBehaviour
            {
                /// <summary>
                ///   Chat users only have a nickname. This will be empty
                ///   when the connection is established but no user is
                ///   authenticated.
                /// </summary>
                public readonly NetworkVariable<string> UserName = new NetworkVariable<string>(new NetworkVariableSettings()
                {
                    ReadPermission = NetworkVariablePermission.Everyone,
                    WritePermission = NetworkVariablePermission.ServerOnly
                });

                private static HashSet<ChatUser> currentUsers = new HashSet<ChatUser>();

                private void Awake()
                {
                    currentUsers.Add(this);
                }

                private void OnDestroy()
                {
                    currentUsers.Remove(this);
                }

                public static ChatUser Owned()
                {
                    foreach(ChatUser user in currentUsers)
                    {
                        if (user.IsOwner) return user;
                    }
                    return null;
                }

                /// <summary>
                ///   Tells whether the user is authenticated.
                /// </summary>
                public bool IsAuthenticated { get { return UserName.Value != ""; } }

                [ServerRpc]
                private void SayServerRpc(string message)
                {
                    if (!IsAuthenticated)
                    {
                        ReceiveResponseClientRpc(new Response() { Success = false, Code = "NOT-AUTHENTICATED" });
                    }
                    else if (message.Trim() == "")
                    {
                        ReceiveResponseClientRpc(new Response() { Success = false, Code = "EMPTY-MESSAGE" });
                    }
                    else
                    {
                        NetworkManager.GetComponent<ChatRealm>().SayOnBehalfOf(this, message);
                        ReceiveResponseClientRpc(new Response() { Success = true, Code = "OK" });
                    }
                }

                public void Say(string message)
                {
                    if (IsClient) SayServerRpc(message);
                }

                [ServerRpc]
                private void JoinServerRpc(string channelName)
                {
                    if (!IsAuthenticated)
                    {
                        ReceiveResponseClientRpc(new Response() { Success = false, Code = "NOT-AUTHENTICATED" });
                    }
                    else
                    {
                        NetworkManager.GetComponent<ChatRealm>().JoinUser(this, channelName);
                        ReceiveResponseClientRpc(new Response() { Success = true, Code = "OK" });
                    }
                }

                public void Join(string channelName)
                {
                    if (IsClient) JoinServerRpc(channelName);
                }

                [ClientRpc]
                private void ReceiveResponseClientRpc(Response response)
                {
                    if (IsOwner)
                    {
                        Debug.LogFormat("Received response: Success={0} Code={1}", response.Success, response.Code);
                    }
                }
            }
        }
    }
}