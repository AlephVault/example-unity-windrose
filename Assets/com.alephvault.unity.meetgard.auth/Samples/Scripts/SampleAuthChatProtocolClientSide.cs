using AlephVault.Unity.Meetgard.Auth.Protocols.Simple;
using AlephVault.Unity.Meetgard.Client;
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
        ///   This is the client side of the authenticated
        ///   chat implementation.
        /// </summary>
        public class SampleAuthChatProtocolClientSide : ProtocolClientSide<SampleAuthChatProtocolDefinition>
        {
            protected override void SetIncomingMessageHandlers()
            {
                throw new NotImplementedException();
            }
        }
    }
}
