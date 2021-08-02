using System;
using System.Threading.Tasks;
using AlephVault.Unity.MMO.Authoring.Behaviours.Authentication;
using AlephVault.Unity.MMO.Authoring.Behaviours.Realms;
using AlephVault.Unity.MMO.Types;
using MLAPI.Serialization;
using UnityEngine;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Connection;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace AlephVault.Unity.MMO.Samples
{
    namespace Behaviours
    {
        namespace Realms
        {
            public class ChatRealm : BasicRealm<string, ChatAccountPreview, ChatAccount>
            {
                /// <summary>
                ///   This class contains a dummy user => password
                ///   authentication base.
                /// </summary>
                [Serializable]
                private class UserPasswordDictionary : Support.Generic.Authoring.Types.Dictionary<string, ChatAccount> { }

#if UNITY_EDITOR
                [CustomPropertyDrawer(typeof(UserPasswordDictionary))]
                public class UserPasswordDictionaryDrawer : Support.Generic.Authoring.Types.DictionaryPropertyDrawer { }
#endif

                /// <summary>
                ///   All of the available accounts.
                /// </summary>
                [SerializeField]
                private UserPasswordDictionary accounts;

                /// <summary>
                ///   All of the setup channels.
                /// </summary>
                [SerializeField]
                private string[] channelNames;

                /// <summary>
                ///   The prefab for a chat room.
                /// </summary>
                [SerializeField]
                private ChatRoom chatRoomPrefab;

                /// <summary>
                ///   The prefab for a chat user.
                /// </summary>
                [SerializeField]
                private ChatUser chatUserPrefab;

                // All of the instantiated channels.
                private Dictionary<string, ChatRoom> channels = new Dictionary<string, ChatRoom>();

                // The related network manager.
                private NetworkManager manager;

                // Whether it was server in the previous frame.
                private bool managerIsServer = false;

                protected new void Awake()
                {
                    base.Awake();
                    manager = GetComponent<NetworkManager>();
                }

                void FixedUpdate()
                {
                    if (managerIsServer != manager.IsServer)
                    {
                        if (manager.IsServer)
                        {
                            foreach(string channelName in channelNames)
                            {
                                ChatRoom room = Instantiate(chatRoomPrefab);
                                Debug.LogFormat("For channel name {0} adding a channel {1}", channelName, room);
                                room.GetComponent<NetworkObject>().Spawn();
                                channels.Add(channelName, room);
                                room.RoomName.Value = channelName;
                            }
                        }
                        else
                        {
                            foreach(ChatRoom room in channels.Values)
                            {
                                room.GetComponent<NetworkObject>().Despawn();
                                Destroy(room);
                            }
                            channels.Clear();
                        }
                    }
                    managerIsServer = manager.IsServer;
                }

                protected override async Task AttendLoginFailure(ulong clientId, Response response, Authenticator.AccountId accountId)
                {
                    Debug.Log(">>> ChatRealm: Login failed");
                }

                protected override async Task ClearAccount(ulong clientId, ChatAccount account)
                {
                    Debug.Log(">>> ChatRealm: Clearing account");
                    ChatUser user = Authenticator.GetSessionData(clientId, "user") as ChatUser;
                    foreach (ChatRoom room in channels.Values)
                    {
                        room.Users.Remove(user);
                    }
                    user.UserName.Value = "";
                }

                protected override async Task InitializeAccount(ulong clientId, ChatAccount account)
                {
                    Debug.Log(">>> ChatRealm: Initializing account");
                    ChatUser user = Instantiate(chatUserPrefab);
                    NetworkObject userObj = user.GetComponent<NetworkObject>();
                    userObj.SpawnWithOwnership(clientId);
                    user.UserName.Value = account.GetPreview().Username;
                    Authenticator.SetSessionData(clientId, "user", user);
                }

                protected override async Task<ChatAccount> LoadAccount(string id)
                {
                    Debug.Log("Loading account by id: " + id);
                    accounts.TryGetValue(id, out ChatAccount account);
                    Debug.LogFormat("Obtained account: {0}", account);
                    return account;
                }

                protected override Tuple<string, Func<NetworkReader, Task<Tuple<Response, object>>>>[] LoginMethods()
                {
                    Debug.Log("Register methods: DUMMY");
                    return new Tuple<string, Func<NetworkReader, Task<Tuple<Response, object>>>>[]
                    {
                        new Tuple<string, Func<NetworkReader, Task<Tuple<Response, object>>>>("DUMMY", async(reader) => {
                            string username = reader.ReadString().ToString();
                            string password = reader.ReadString().ToString();
                            Debug.Log("Receiving DUMMY login");
                            try
                            {
                                ChatAccount account = accounts[username];
                                if (account.Matches(password))
                                {
                                    return new Tuple<Response, object>(new Response() { Success = true, Code = "OK" }, username);
                                }
                                else
                                {
                                    return new Tuple<Response, object>(new Response() { Success = false, Code = "Mismatch" }, username);
                                }
                            }
                            catch(KeyNotFoundException)
                            {
                                return new Tuple<Response, object>(new Response() { Success = false, Code = "Unknown" }, username);
                            }
                        })
                    };
                }

                public void DummyLogin(string username, string password)
                {
                    Authenticator.Authenticate("DUMMY", (writer) =>
                    {
                        writer.WriteString(username);
                        writer.WriteString(password);
                    });
                }

                protected override string Name()
                {
                    return "DummyChat";
                }

                public bool SayOnBehalfOf(ChatUser user, string message)
                {
                    if (user.IsAuthenticated)
                    {
                        foreach (ChatRoom room in channels.Values)
                        {
                            if (room.Users.Contains(user))
                            {
                                room.Messages.Add(new ChatRoom.ChatMessage() { UserName = user.UserName.Value, Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Body = message });
                            }
                        }
                        return true;
                    }
                    return false;
                }

                public bool JoinUser(ChatUser user, string channelName)
                {
                    if (user.IsAuthenticated)
                    {
                        foreach (ChatRoom room in channels.Values)
                        {
                            if (room.Users.Contains(user) && room.RoomName.Value != channelName)
                            {
                                room.Users.Remove(user);
                                room.Messages.Add(new ChatRoom.ChatMessage() { UserName = "[Server]", Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Body = user.UserName.Value + " left" });
                            }
                            if (!room.Users.Contains(user) && room.RoomName.Value == channelName)
                            {
                                room.Users.Add(user);
                                room.Messages.Add(new ChatRoom.ChatMessage() { UserName = "[Server]", Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Body = user.UserName.Value + " joined" });
                            }
                        }
                        return true;
                    }
                    return false;
                }
            }
        }
    }
}
