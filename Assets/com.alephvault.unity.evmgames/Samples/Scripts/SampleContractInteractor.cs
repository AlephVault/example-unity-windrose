using System;
using System.Collections.Generic;
using AlephVault.Unity.EVMGames.Nethereum.Contracts;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;
using AlephVault.Unity.EVMGames.Samples.EthModels;
using AlephVault.Unity.Support.Utils;
using UnityEngine;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;


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
            }

            private void Start()
            {
                StartWeb3Clients();
                StartTabs();
                StartEventsPanel();
            }
        }
    }
}
