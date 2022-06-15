using System;
using UnityEngine;
using UnityEngine.UI;


namespace AlephVault.Unity.EVMGames
{
    namespace Samples
    {
        public partial class SampleContractInteractor
        {
            private Text currentPrivateKey;
            private InputField chosenAddressInput;

            private Button directBalanceOfButton;
            private InputField directBalanceOfInput;
            private Text directBalanceOfResult;

            private InputField directSendTokensToInput;
            private InputField directSendTokensAmountInput;
            private Button directSendTokensButton;
            private Text directSendTokensResult;
            
            private void AwakeDirectWallet()
            {
                currentPrivateKey = transform.Find("pkWalletPanel/currentPrivateKey").GetComponent<Text>();
                chosenAddressInput = transform.Find("pkWalletPanel/chosenAddressInput").GetComponent<InputField>();

                directBalanceOfButton = transform.Find("pkWalletPanel/balanceOfButton").GetComponent<Button>();
                directBalanceOfInput = transform.Find("pkWalletPanel/balanceOfInput").GetComponent<InputField>();
                directBalanceOfResult = transform.Find("pkWalletPanel/balanceOfResult").GetComponent<Text>();
                
                directSendTokensToInput = transform.Find("pkWalletPanel/sendTokensToInput").GetComponent<InputField>();
                directSendTokensAmountInput = transform.Find("pkWalletPanel/sendTokensAmountInput").GetComponent<InputField>();
                directSendTokensButton = transform.Find("pkWalletPanel/sendTokensButton").GetComponent<Button>();
                directSendTokensResult = transform.Find("pkWalletPanel/sendTokensResult").GetComponent<Text>();
            }
            
            private async void StartDirectWallet()
            {
                try
                {
                    currentPrivateKey.text = $"{privateKey}";
                    chosenAddressInput.text = web3DirectClient.TransactionManager.Account.Address;
                    directBalanceOfButton.onClick.AddListener( () =>
                    {
                        DoBalanceOf(web3DirectClient, directBalanceOfInput, directBalanceOfResult);
                    });
                    directSendTokensButton.onClick.AddListener(() =>
                    {
                        DoTransfer(web3DirectClient, chosenAddressInput, directSendTokensToInput, directSendTokensAmountInput, directSendTokensResult);
                    });
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
    }
}
