using System;
using System.Linq;
using MLAPI;
using MLAPI.NetworkVariable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI.NetworkVariable.Collections;

namespace AlephVault.Unity.MMO.Samples
{
    namespace Behaviours
    {
        namespace Chat
        {
            public class Channel : NetworkBehaviour
            {
                // All of the available channels, by their names.
                private static Dictionary<string, Channel> allChannels = new Dictionary<string, Channel>();

                // A list of all the users currently in the room.
                private HashSet<User> users = new HashSet<User>();

                // A synchronized list of all the user names.
                private NetworkList<string> userNames = new NetworkList<string>(new NetworkVariableSettings()
                {
                    WritePermission = NetworkVariablePermission.ServerOnly,
                    ReadPermission = NetworkVariablePermission.Everyone
                });

                // A list of all the messages that have been said.
                private NetworkList<Tuple<string, string>> messages = new NetworkList<Tuple<string, string>>(new NetworkVariableSettings()
                {
                    WritePermission = NetworkVariablePermission.ServerOnly,
                    ReadPermission = NetworkVariablePermission.Everyone
                });

                [SerializeField]
                private string channelName;

                private void Start()
                {
                    allChannels.Add(channelName, this);
                    userNames.OnListChanged += UserNames_OnListChanged;
                    messages.OnListChanged += Messages_OnListChanged;
                }

                private void OnDestroy()
                {
                    messages.OnListChanged -= Messages_OnListChanged;
                    userNames.OnListChanged -= UserNames_OnListChanged;
                    users.Clear();
                    allChannels.Remove(channelName);
                }

                private void UserNames_OnListChanged(NetworkListEvent<string> changeEvent)
                {
                    // TODO refreshi ui.
                }

                private void Messages_OnListChanged(NetworkListEvent<Tuple<string, string>> changeEvent)
                {
                    // TODO refresh ui.
                }

                /// <summary>
                ///   Tells whether the current channel is alive (i.e. between initialization
                ///   and destruction).
                /// </summary>
                public bool IsAlive()
                {
                    return allChannels.ContainsValue(this);
                }

                /// <summary>
                ///   Finds a channel by its name, or returns null of not found.
                /// </summary>
                /// <param name="name">The name to find a channel by</param>
                /// <returns>The result - either a channel, or null.</returns>
                public static Channel Find(string name)
                {
                    Channel result;
                    allChannels.TryGetValue(name, out result);
                    return result;
                }

                /// <summary>
                ///   Says something to this channel on behalf of a user.
                /// </summary>
                /// <param name="username">The user</param>
                /// <param name="message">What to say</param>
                public void SayOnBehalf(string username, string message)
                {
                    messages.Add(new Tuple<string, string>(username, message));
                }

                /// <summary>
                ///   Adds a user to the channel.
                /// </summary>
                /// <param name="user">The user to add</param>
                /// <returns>Whether it was added or already existed there</returns>
                public bool JoinUser(User user)
                {
                    bool result = users.Add(user);
                    RefreshNicknames();
                    return result;
                }

                /// <summary>
                ///   Removes a user to the channel.
                /// </summary>
                /// <param name="user">The user to remove</param>
                /// <returns>Whether it was removed, or already wasn't there</returns>
                public bool LeaveUser(User user)
                {
                    bool result = users.Remove(user);
                    RefreshNicknames();
                    return result;
                }

                /// <summary>
                ///   Refreshes all of the nicknames.
                /// </summary>
                public void RefreshNicknames()
                {
                    userNames.Clear();
                    foreach(User user in users)
                    {
                        userNames.Add(user.Nickname);
                    }
                }

                /// <summary>
                ///   Returns all the usernames.
                /// </summary>
                /// <returns>The names of the users in the channel.</returns>
                public List<string> UserNames()
                {
                    return userNames.ToList();
                }

                /// <summary>
                ///   Returns all the messages.
                /// </summary>
                /// <returns>The messages</returns>
                public List<Tuple<string, string>> Messages()
                {
                    return messages.ToList();
                }
            }
        }
    }
}