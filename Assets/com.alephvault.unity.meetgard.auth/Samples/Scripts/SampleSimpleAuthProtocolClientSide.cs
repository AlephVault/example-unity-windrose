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
        public class SampleSimpleAuthProtocolClientSide : SimpleAuthProtocolClientSide<SampleSimpleAuthProtocolDefinition, Nothing, LoginFailed, Kicked>
        {
            /// <summary>
            ///   This is a sample login method with
            ///   username and password.
            /// </summary>
            private Func<UserPass, Task> SendSampleLogin;

            /// <summary>
            ///   The sample username.
            /// </summary>
            [SerializeField]
            private string Username;

            /// <summary>
            ///   The sample password.
            /// </summary>
            [SerializeField]
            private string Password;

            protected new void Awake()
            {
                base.Awake();
                OnWelcome += SampleSimpleAuthProtocolClientSide_OnWelcome;
            }

            void OnDestroy()
            {
                OnWelcome -= SampleSimpleAuthProtocolClientSide_OnWelcome;
            }

            private async Task SampleSimpleAuthProtocolClientSide_OnWelcome()
            {
                await SendSampleLogin(new UserPass() { Username = Username, Password = Password });
            }

            protected override void MakeLoginRequestSenders()
            {
                SendSampleLogin = MakeLoginRequestSender<UserPass>("Sample");
            }
        }
    }
}
