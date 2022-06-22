using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Auth.Protocols;
using AlephVault.Unity.EVMGames.Auth.Types;
using AlephVault.Unity.Meetgard.Auth.Samples;
using UnityEngine;

namespace AlephVault.Unity.EVMGames.Auth
{
    namespace Samples
    {
        public class SampleEVMAuthProtocolClientSide : EVMAuthProtocolClientSide<EVMLoginFailed, Kicked>
        {
            protected override void Setup()
            {
                base.Setup();
                OnWelcome += SampleEVMAuthProtocolClientSide_OnWelcome;
                OnLoggedOut += SampleEVMAuthProtocolClientSide_OnLoggedOut;
                OnLoginFailed += SampleEVMAuthProtocolClientSide_OnLoginFailed;
            }
            
            protected void OnDestroy()
            {
                OnWelcome -= SampleEVMAuthProtocolClientSide_OnWelcome;
                OnLoggedOut -= SampleEVMAuthProtocolClientSide_OnLoggedOut;
                OnLoginFailed -= SampleEVMAuthProtocolClientSide_OnLoginFailed;
            }

            private async Task SampleEVMAuthProtocolClientSide_OnWelcome()
            {
                Debug.Log("Sample EVM Client::Logged in");
            }
            
            private async Task SampleEVMAuthProtocolClientSide_OnLoggedOut()
            {
                Debug.Log("Sample EVM Client::Logged out");
            }
            
            private async Task SampleEVMAuthProtocolClientSide_OnLoginFailed(EVMLoginFailed arg)
            {
                Debug.Log($"Sample EVM Client::Login failed: {arg.Reason}");
            }
        }
    }
}