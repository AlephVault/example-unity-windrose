using System;
using MLAPI;
using MLAPI.NetworkVariable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI.NetworkVariable.Collections;
using MLAPI.Messaging;

namespace AlephVault.Unity.MMO.Samples
{
    namespace Behaviours
    {
        namespace Chat
        {
            public class User : NetworkBehaviour
            {
                // Channels the user belongs to.
                private HashSet<Channel> channels = new HashSet<Channel>();

                // Nickname (which will be synchronized) of the user.
                private NetworkVariable<string> nickname = new NetworkVariable<string>(
                    new NetworkVariableSettings()
                    {
                        ReadPermission = NetworkVariablePermission.Everyone,
                        WritePermission = NetworkVariablePermission.ServerOnly
                    }
                );

                // RPC call to change the nick in server.
                [ServerRpc(RequireOwnership = true)]
                private void SetNicknameServerRpc(string newNickname)
                {
                    newNickname = newNickname.Trim();
                    if (newNickname != "")
                    {
                        nickname.Value = newNickname;
                        foreach(Channel channel in channels)
                        {
                            channel.RefreshNicknames();
                        }
                    }
                }

                // RPC call to say something in a channel.
                [ServerRpc(RequireOwnership = true)]
                private void SayServerRpc(string message, string channelName)
                {
                    message = message.Trim();
                    Channel channel = Channel.Find(channelName);
                    if (message != "" && channel != null)
                    {
                        channel.SayOnBehalf(nickname.Value, message);
                    }
                }

                // RPC call to join a channel.
                [ServerRpc(RequireOwnership = true)]
                private void JoinChannelServerRpc(string channelName)
                {
                    Channel channel = Channel.Find(channelName);
                    if (channel != null)
                    {
                        channel.JoinUser(this);
                        channels.Add(channel);
                    }
                }

                // RPC call to leave a channel.
                [ServerRpc(RequireOwnership = true)]
                private void LeaveChannelServerRpc(string channelName)
                {
                    Channel channel = Channel.Find(channelName);
                    if (channel != null)
                    {
                        channel.LeaveUser(this);
                        channels.Remove(channel);
                    }
                }

                /// <summary>
                ///   Says something to a particular channel it belongs to.
                /// </summary>
                public void Say(string message, string channel)
                {
                    if (IsClient)
                    {
                        SayServerRpc(message, channel);
                    }
                }

                /// <summary>
                ///   Joins a chosen channel.
                /// </summary>
                /// <param name="channel">The name of the channel to join</param>
                public void Join(string channel)
                {
                    if (IsClient)
                    {
                        JoinChannelServerRpc(channel);
                    }
                }

                /// <summary>
                ///   Leaves a chosen channel it belongs to.
                /// </summary>
                /// <param name="channel">The name of the channel to leave</param>
                public void Leave(string channel)
                {
                    if (IsClient)
                    {
                        LeaveChannelServerRpc(channel);
                    }
                }

                /// <summary>
                ///   Returns the nickname of the user.
                /// </summary>
                public string Nickname
                {
                    get { return nickname.Value; }
                    set
                    {
                        if (IsClient)
                        {
                            SetNicknameServerRpc(value);
                        }
                    }
                }
            }
        }
    }
}