using AlephVault.Unity.Meetgard.Server;
using AlephVault.Unity.Support.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace AlephVault.Unity.Meetgard.Samples
{
    namespace Chat
    {
        public class ChatProtocolServerSide : ProtocolServerSide<ChatProtocolDefinition>
        {
            [SerializeField]
            private float nicknameTimeout = 2f;

            [SerializeField]
            private float pingTimeout = 10f;

            private float pingStart = 50f;
            private float pingCycle = 60f;
            private float currentPingTime = 0;

            private Dictionary<ulong, string> Nicknames = new Dictionary<ulong, string>();
            private HashSet<ulong> Connections = new HashSet<ulong>();
            private Dictionary<ulong, float> Timeouts = new Dictionary<ulong, float>();

            private Func<ulong, Task> SendWhoAreYou;
            private Func<ulong, Task> SendNicknameOK;
            private Func<ulong, Task> SendNicknameDuplicated;
            private Func<ulong, Task> SendNicknameAlreadyIntroduced;
            private Func<ulong, Task> SendSayOK;
            private Func<ulong, Task> SendSayNotIntroduced;
            private Action<ulong[], Said, Dictionary<ulong, Task>> BroadcastSaid;
            private Action<ulong[], Nickname, Dictionary<ulong, Task>> BroadcastJoined;
            private Action<ulong[], Nickname, Dictionary<ulong, Task>> BroadcastLeft;
            private Func<ulong, Task> SendPingTimeout;
            private Action<ulong[], Echo, Dictionary<ulong, Task>> BroadcastPing;

            protected new void Awake()
            {
                base.Awake();
                nicknameTimeout = Values.Clamp(1f, nicknameTimeout, 5f);
                pingTimeout = Values.Clamp(10f, pingTimeout, 20f);
                pingCycle = pingTimeout * 6f;
                pingStart = pingTimeout * 5f;
                SendWhoAreYou = MakeSender("WhoAreYou");
                SendNicknameOK = MakeSender("Nickname:OK");
                SendNicknameDuplicated = MakeSender("Nickname:Duplicated");
                SendNicknameAlreadyIntroduced = MakeSender("Nickname:AlreadyIntroduced");
                SendSayOK = MakeSender("Say:OK");
                SendSayNotIntroduced = MakeSender("Say:NotIntroduced");
                BroadcastSaid = MakeBroadcaster<Said>("Say:Said");
                BroadcastJoined = MakeBroadcaster<Nickname>("Nickname:Joined");
                BroadcastLeft = MakeBroadcaster<Nickname>("Nickname:Left");
                SendPingTimeout = MakeSender("Ping:Timeout");
                BroadcastPing = MakeBroadcaster<Echo>("Ping");
            }

            private void Update()
            {
                bool wasInPingRange = currentPingTime > pingStart;
                currentPingTime += Time.unscaledDeltaTime;
                bool isInPingRange = currentPingTime > pingStart;

                if (!wasInPingRange && isInPingRange)
                {
                    foreach (ulong clientId in Connections)
                    {
                        Timeouts[clientId] = currentPingTime - pingStart;
                    }
                    BroadcastPing(null, new Echo() { Content = "Lalala" }, null);
                }
                else if (isInPingRange)
                {
                    foreach (ulong clientId in Timeouts.Keys.ToArray())
                    {
                        if (Timeouts[clientId] > pingTimeout)
                        {
                            TimeoutKick(clientId);
                            Timeouts.Remove(clientId);
                        }
                        else
                        {
                            Timeouts[clientId] += Time.unscaledDeltaTime;
                        }
                    }
                }
                if (currentPingTime > pingCycle)
                {
                    currentPingTime -= pingCycle;
                }
            }

            private async void TimeoutKick(ulong clientId)
            {
                await SendPingTimeout(clientId);
                Debug.Log($"server :: client({clientId}) timed out");
                server.Close(clientId);
            }

            public override async Task OnConnected(ulong clientId)
            {
                Connections.Add(clientId);
                Debug.Log($"server :: client({clientId}) connected");
                await SendWhoAreYou(clientId);
                Debug.Log($"server :: server >>> WhoAreYou >>> client({clientId})");
            }

            public override async Task OnDisconnected(ulong clientId, Exception reason)
            {
                Connections.Remove(clientId);
                Timeouts.Remove(clientId);
                if (Nicknames.TryGetValue(clientId, out string nickname))
                {
                    Nicknames.Remove(clientId);
                    BroadcastLeft(null, new Nickname() { Nick = nickname }, null);
                    Debug.Log($"server :: server >>> Nickname:Left({nickname}) >>> all");
                }
            }

            protected override void SetIncomingMessageHandlers()
            {
                AddIncomingMessageHandler<Nickname>("Nickname", async (proto, clientId, nick) =>
                {
                    Debug.Log($"server :: client({clientId}) >>> Nickname({nick}) >>> server");
                    if (Nicknames.ContainsKey(clientId))
                    {
                        await SendNicknameAlreadyIntroduced(clientId);
                        Debug.Log($"server :: server >>> Nickname:AlreadyIntroduced >>> client({clientId})");
                    }
                    else if (Nicknames.ContainsValue(nick.Nick))
                    {
                        await SendNicknameDuplicated(clientId);
                        Debug.Log($"server :: server >>> Nickname:Duplicated >>> client({clientId})");
                        server.Close(clientId);
                    }
                    else
                    {
                        Nicknames.Add(clientId, nick.Nick);
                        await SendNicknameOK(clientId);
                        Debug.Log($"server :: server >>> Nickname:OK >>> client({clientId})");
                        BroadcastJoined(null, nick, null);
                        Debug.Log($"server :: server >>> Nickname:Joined({nick}) >>> all");
                    }
                });
                AddIncomingMessageHandler<Line>("Say", async (proto, clientId, line) =>
                {
                    Debug.Log($"server :: client({clientId}) >>> Say({line}) >>> server");
                    if (Nicknames.TryGetValue(clientId, out string nick))
                    {
                        await SendSayOK(clientId);
                        Debug.Log($"server :: server >>> Say:OK >>> client({clientId})");
                        BroadcastSaid(null, new Said() { Nickname = nick, Content = line.Content, When = DateTime.Now.ToString("F") }, null);
                        Debug.Log($"server :: server >>> Say:Said({nick}, {line.Content}) >>> all");
                    }
                    else
                    {
                        await SendSayNotIntroduced(clientId);
                        Debug.Log($"server :: server >>> Say:NotIntroduced >>> client({clientId})");
                    }
                });
            }
        }
    }
}