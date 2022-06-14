using System.Collections.Generic;
using AlephVault.Unity.EVMGames.Nethereum.Contracts;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;
using AlephVault.Unity.EVMGames.Samples.EthModels;
using AlephVault.Unity.Support.Utils;
using UnityEngine;
using UnityEngine.UI;


namespace AlephVault.Unity.EVMGames
{
    namespace Samples
    {
        public partial class SampleContractInteractor
        {
            private Text currentPrivateKey;
            private InputField directAddressesBox;

            private Button balanceOfButton;
            private InputField balanceOfInput;
            private Text balanceOfResult;

            private InputField sendTokensToInput;
            private InputField sendTokensAmountInput;
            private Button sendTokensButton;
            private Text sendTokensResult;
            
            private void AwakeDirectWallet()
            {
                currentPrivateKey = transform.Find("pkWalletPanel/currentPrivateKey").GetComponent<Text>();
                directAddressesBox = transform.Find("pkWalletPanel/addressesBox").GetComponent<InputField>();

                balanceOfButton = transform.Find("pkWalletPanel/balanceOfButton").GetComponent<Button>();
                balanceOfInput = transform.Find("pkWalletPanel/balanceOfInput").GetComponent<InputField>();
                balanceOfResult = transform.Find("pkWalletPanel/balanceOfResult").GetComponent<Text>();
                
                sendTokensToInput = transform.Find("pkWalletPanel/sendTokensToInput").GetComponent<InputField>();
                sendTokensToInput = transform.Find("pkWalletPanel/sendTokensAmountInput").GetComponent<InputField>();
                sendTokensButton = transform.Find("pkWalletPanel/sendTokensButton").GetComponent<Button>();
                sendTokensResult = transform.Find("pkWalletPanel/sendTokensResult").GetComponent<Text>();
            }
            
            private async void StartDirectWallet()
            {
                directAddressesBox.text = string.Join("\n", await web3DirectClient.Eth.Accounts.SendRequestAsync());
                balanceOfButton.onClick.AddListener( () =>
                {
                    DoBalanceOf(web3DirectClient, balanceOfInput, balanceOfResult);
                });
                sendTokensButton.onClick.AddListener(() =>
                {
                    DoTransfer(web3DirectClient, sendTokensToInput, sendTokensAmountInput, sendTokensResult);
                });
            }
        }
    }
}
