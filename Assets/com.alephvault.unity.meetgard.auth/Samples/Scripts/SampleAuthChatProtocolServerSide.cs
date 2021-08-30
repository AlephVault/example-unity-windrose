using AlephVault.Unity.Meetgard.Auth.Protocols.Simple;
using AlephVault.Unity.Meetgard.Samples.Chat;
using AlephVault.Unity.Meetgard.Server;
using AlephVault.Unity.Meetgard.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace AlephVault.Unity.Meetgard.Auth
{
    namespace Samples
    {
        /// <summary>
        ///   This is the server side of the authenticated
        ///   chat implementation.
        /// </summary>
        [RequireComponent(typeof(SampleSimpleAuthProtocolServerSide))]
        public class SampleAuthChatProtocolServerSide : ProtocolServerSide<SampleAuthChatProtocolDefinition>
        {
            SampleSimpleAuthProtocolServerSide authProtocol;

            private Dictionary<ulong, SampleAccount> users = new Dictionary<ulong, SampleAccount>();

            private Func<IEnumerable<ulong>, SampleAccountPreview, Dictionary<ulong, Task>> SendJoined;
            private Func<IEnumerable<ulong>, SampleAccountPreview, Dictionary<ulong, Task>> SendLeft;
            private Func<IEnumerable<ulong>, Said, Dictionary<ulong, Task>> SendSaid;

            protected new void Awake()
            {
                base.Awake();
                authProtocol = GetComponent<SampleSimpleAuthProtocolServerSide>();
            }

            protected void Start()
            {
                authProtocol.OnSessionStarting += AuthProtocol_OnSessionStarting;
                authProtocol.OnSessionTerminating += AuthProtocol_OnSessionTerminating;
                SendJoined = MakeBroadcaster<SampleAccountPreview>("Joined");
                SendLeft = MakeBroadcaster<SampleAccountPreview>("Left");
                SendSaid = MakeBroadcaster<Said>("Said");
            }

            protected void OnDestroy()
            {
                authProtocol.OnSessionStarting -= AuthProtocol_OnSessionStarting;
                authProtocol.OnSessionTerminating -= AuthProtocol_OnSessionTerminating;
            }

            private async Task AuthProtocol_OnSessionStarting(ulong arg1, SampleAccount arg2)
            {
                users.Add(arg1, arg2);
                await UntilBroadcastIsDone(SendJoined(users.Keys, arg2.GetProfileDisplayData()));
            }

            private async Task AuthProtocol_OnSessionTerminating(ulong arg1, Kicked arg2)
            {
                SampleAccount account = users[arg1];
                users.Remove(arg1);
                await UntilBroadcastIsDone(SendLeft(users.Keys, account.GetProfileDisplayData()));
            }

            protected override void SetIncomingMessageHandlers()
            {
                AddIncomingMessageHandler<Line>("Say", authProtocol.LoginRequired<SampleAuthChatProtocolDefinition, Line>((proto, clientId, message) => {
                    return UntilBroadcastIsDone(SendSaid(users.Keys, new Said() { Nickname = users[clientId].Username, Content = message.Content, When = DateTime.Now.ToString("F") }));
                }));
            }
        }
    }
}
