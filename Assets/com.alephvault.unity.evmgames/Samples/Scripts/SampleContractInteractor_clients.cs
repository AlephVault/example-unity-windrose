using System;
using AlephVault.Unity.EVMGames.WalletConnectSharp.NEthereum;
using AlephVault.Unity.EVMGames.Nethereum.Web3;
using AlephVault.Unity.EVMGames.Nethereum.Web3.Accounts;
using AlephVault.Unity.EVMGames.WalletConnectSharp.Unity;
using UnityEngine;


namespace AlephVault.Unity.EVMGames
{
    namespace Samples
    {
        public partial class SampleContractInteractor
        {
            private Web3 web3DirectClient;
            private Web3 web3IndirectClient;

            private void InitWeb3DirectClient()
            {
                // Initializes the direct client from a private key.
                // It also uses the provided URL. This client will
                // permanently exist while this app run, and the
                // events will be read by using this direct client.
                //
                // This function is called only once, on initialization.
                web3DirectClient = new Web3(new Account(privateKey, ChainId), endpoint);
            }

            private void InitWeb3IndirectClient()
            {
                // Initializes the indirect client from a session.
                // The session is not null by this point, and will
                // be valid (connected). This function is called
                // when a new session is established.
                web3IndirectClient = new Web3(WalletConnect.ActiveSession.CreateProvider(new Uri(endpoint)));
            }

            private void ClearWeb3IndirectClient(WalletConnectUnitySession session)
            {
                // Clears the indirect client. This function is
                // called when a session is terminated.
                web3IndirectClient = null;
            }

            private void InitWeb3Clients()
            {
                // This function requires the walletConnect instance
                // to appropriately exist.
                InitWeb3DirectClient();
                WalletConnect.Instance.ConnectedEvent.AddListener(InitWeb3IndirectClient);
                WalletConnect.Instance.DisconnectedEvent.AddListener(ClearWeb3IndirectClient);
            }
        }
    }
}
