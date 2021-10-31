using AlephVault.Unity.Binary.Wrappers;
using AlephVault.Unity.Meetgard.Authoring.Behaviours.Server;
using AlephVault.Unity.Meetgard.Scopes.Authoring.Behaviours.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


namespace AlephVault.Unity.Meetgard.Scopes
{
    namespace Samples
    {
        [RequireComponent(typeof(ScopesProtocolServerSide))]
        public class SampleProtocolServerSide : ProtocolServerSide<SampleProtocolDefinition>
        {
            private ScopesProtocolServerSide scopesProtocol;
            private Func<ulong, Task> SendOK;

            protected override void Initialize()
            {
                scopesProtocol = GetComponent<ScopesProtocolServerSide>();
                SendOK = MakeSender("OK");
            }

            protected override void SetIncomingMessageHandlers()
            {
                AddIncomingMessageHandler("GoTo:Extra", async (proto, connectionId) => {
                    ScopeServerSide sss = await scopesProtocol.LoadExtraScope("sample-extra");
                    await scopesProtocol.SendTo(connectionId, sss.Id);
                    await SendOK(connectionId);
                });

                AddIncomingMessageHandler("GoTo:Limbo", async (proto, connectionId) => {
                    await scopesProtocol.SendToLimbo(connectionId);
                    await SendOK(connectionId);
                });

                AddIncomingMessageHandler<UInt>("GoTo:Default", async (proto, connectionId, message) => {
                    await scopesProtocol.SendTo(connectionId, message);
                    await SendOK(connectionId);
                });
            }
        }
    }
}