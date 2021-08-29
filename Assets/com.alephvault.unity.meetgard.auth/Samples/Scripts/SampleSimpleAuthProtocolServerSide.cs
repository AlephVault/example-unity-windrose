using AlephVault.Unity.Meetgard.Auth.Protocols.Simple;
using AlephVault.Unity.Meetgard.Auth.Types;
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
            /// <summary>
            ///   The list of valid accounts.
            /// </summary>
            [SerializeField]
            private Dictionary<string, SampleAccount> accounts = new Dictionary<string, SampleAccount>();

            /// <summary>
            ///   The duplicate account management mode.
            /// </summary>
            [SerializeField]
            private MultipleSessionsManagementMode MultipleSessionsManagementMode = MultipleSessionsManagementMode.Reject;

            protected override async Task<SampleAccount> FindAccount(string id)
            {
                foreach (var pair in accounts)
                {
                    if (id == pair.Key.Trim().ToLower())
                    {
                        return pair.Value;
                    }
                }
                return null;
            }

            protected override MultipleSessionsManagementMode IfAccountAlreadyLoggedIn()
            {
                return MultipleSessionsManagementMode;
            }

            protected override async Task OnSessionError(ulong clientId, SessionStage stage, System.Exception error)
            {
                Debug.Log($"Exception on session stage {stage} for client id {clientId}: {error.GetType().FullName} - {error.Message}");
            }

            protected override void SetLoginMessageHandlers()
            {
                AddLoginMessageHandler<UserPass>("Sample", async (message) =>
                {
                    foreach(var pair in  accounts)
                    {
                        if (message.Username.Trim().ToLower() == pair.Key.Trim().ToLower() && message.Password == pair.Value.Password)
                        {
                            return new Tuple<bool, Nothing, LoginFailed, string>(true, new Nothing(), null, message.Username);
                        }
                    }
                    return new Tuple<bool, Nothing, LoginFailed, string>(false, null, new LoginFailed(), "");
                });
            }
        }
    }
}
