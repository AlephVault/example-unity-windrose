using AlephVault.Unity.Meetgard.Auth.Protocols.Simple;
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
        ///   This sample login protocol has only one mean
        ///   of login: "Login:Sample" (Username, Password).
        /// </summary>
        public class SampleSimpleAuthProtocolServerSide : SimpleAuthProtocolServerSide<SampleSimpleAuthProtocolDefinition, Nothing, LoginFailed, Kicked, string, SampleAccountPreview, SampleAccount>
        {
            protected override Task<SampleAccount> FindAccount(string id)
            {
                throw new NotImplementedException();
            }

            protected override Task OnSessionError(ulong clientId, SessionStage stage, System.Exception error)
            {
                throw new NotImplementedException();
            }

            protected override Task OnSessionStarting(ulong clientId, SampleAccount accountData)
            {
                throw new NotImplementedException();
            }

            protected override Task OnSessionTerminating(ulong clientId, Kicked reason)
            {
                throw new NotImplementedException();
            }

            protected override void SetLoginMessageHandlers()
            {
                AddLoginMessageHandler<UserPass>("Sample", async (message) =>
                {
                    // TODO implement this
                    return new Tuple<bool, Nothing, LoginFailed, string>(true, new Nothing(), new LoginFailed(), "");
                });
            }
        }
    }
}
