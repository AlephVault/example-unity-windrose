using AlephVault.Unity.Binary.Wrappers;
using AlephVault.Unity.Meetgard.Authoring.Behaviours.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


namespace AlephVault.Unity.Meetgard.Scopes
{
    namespace Samples
    {
        public class SampleProtocolClientSide : ProtocolClientSide<SampleProtocolDefinition>
        {
            private Func<Task> SendGoToExtra;
            private Func<Task> SendGoToLimbo;
            private Func<UInt, Task> SendGoToDefault;

            protected override void Initialize()
            {
                SendGoToExtra = MakeSender("GoTo:Extra");
                SendGoToLimbo = MakeSender("GoTo:Limbo");
                SendGoToDefault = MakeSender<UInt>("GoTo:Default");
            }

            public async void DoSendGoToLimbo()
            {
                await SendGoToLimbo();
            }

            public async void DoSendGoToExtra()
            {
                await SendGoToExtra();
            }

            public async void DoSendGoToDefault(uint index)
            {
                await SendGoToDefault((UInt)index);
            }

            protected override void SetIncomingMessageHandlers()
            {
                AddIncomingMessageHandler("OK", async (proto) => {
                    Debug.Log("Success on sample request");
                });
            }
        }
    }
}