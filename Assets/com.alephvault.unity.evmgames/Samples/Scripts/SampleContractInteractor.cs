using UnityEngine;


namespace AlephVault.Unity.EVMGames
{
    namespace Samples
    {
        public partial class SampleContractInteractor : MonoBehaviour
        {
            // This is a particular test done in Mumbai.
            private const string ContractAddress = "0xe2981920E60195AbEfE3F26948a365c70F21615C";
            private const int ChainId = 80001;
            
            // Private key and endpoint go here.

            [SerializeField]
            private string privateKey;

            [SerializeField]
            private string endpoint;
            
            private void Awake()
            {
                AwakeTabs();
                AwakeEventsPanel();
                AwakeDirectWallet();
                AwakeIndirectWallet();
            }

            private void Start()
            {
                StartWeb3Clients();
                StartTabs();
                StartEventsPanel();
                StartDirectWallet();
                StartIndirectWallet();
            }
        }
    }
}
