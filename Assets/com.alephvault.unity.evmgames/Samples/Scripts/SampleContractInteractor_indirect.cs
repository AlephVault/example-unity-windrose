using System;
using AlephVault.Unity.EVMGames.WalletConnectSharp.Unity;
using UnityEngine;
using UnityEngine.UI;


namespace AlephVault.Unity.EVMGames
{
    namespace Samples
    {
        public partial class SampleContractInteractor
        {
            private Image indirectQRImage;
            private bool connecting = false;
            private bool connected = false;
            
            private Text indirectConnectionStatusLabel;
            private InputField indirectAddressesBox;

            private Button indirectBalanceOfButton;
            private InputField indirectBalanceOfInput;
            private Text indirectBalanceOfResult;

            private InputField indirectSendTokensToInput;
            private InputField indirectSendTokensAmountInput;
            private Button indirectSendTokensButton;
            private Text indirectSendTokensResult;

            private Button indirectConnectButton;
            private Button indirectDisconnectButton;

            private void AwakeIndirectWallet()
            {
                indirectQRImage = transform.Find("connectedWalletPanel/balanceOfButton").GetComponent<Image>();
                indirectConnectionStatusLabel = transform.Find("connectedWalletPanel/statusLabel").GetComponent<Text>();
                indirectAddressesBox = transform.Find("connectedWalletPanel/addressesBox").GetComponent<InputField>();

                indirectBalanceOfButton = transform.Find("connectedWalletPanel/balanceOfButton").GetComponent<Button>();
                indirectBalanceOfInput = transform.Find("connectedWalletPanel/balanceOfInput").GetComponent<InputField>();
                indirectBalanceOfResult = transform.Find("connectedWalletPanel/balanceOfResult").GetComponent<Text>();
                
                indirectSendTokensToInput = transform.Find("connectedWalletPanel/sendTokensToInput").GetComponent<InputField>();
                indirectSendTokensAmountInput = transform.Find("connectedWalletPanel/sendTokensAmountInput").GetComponent<InputField>();
                indirectSendTokensButton = transform.Find("connectedWalletPanel/sendTokensButton").GetComponent<Button>();
                indirectSendTokensResult = transform.Find("connectedWalletPanel/sendTokensResult").GetComponent<Text>();
                
                indirectConnectButton = transform.Find("connectedWalletPanel/connectButton").GetComponent<Button>();
                indirectDisconnectButton = transform.Find("connectedWalletPanel/disconnectButton").GetComponent<Button>();
            }
            
            private void StartIndirectWallet()
            {
                indirectBalanceOfButton.onClick.AddListener( () =>
                {
                    DoBalanceOf(web3IndirectClient, indirectBalanceOfInput, indirectBalanceOfResult);
                });
                indirectSendTokensButton.onClick.AddListener(() =>
                {
                    DoTransfer(web3IndirectClient, indirectSendTokensToInput, indirectSendTokensAmountInput, indirectSendTokensResult);
                });
                indirectConnectButton.onClick.AddListener(async () =>
                {
                    try
                    {
                        await WalletConnect.Instance.Connect();
                    }
                    catch (Exception e)
                    {
                        indirectConnectButton.interactable = true;
                        Debug.LogException(e);
                    }
                });
                indirectDisconnectButton.onClick.AddListener(() =>
                {
                    try
                    {
                        WalletConnect.Instance.CloseSession();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                });

                WalletConnect.Instance.ConnectionStarted += (sender, args) =>
                {
                    indirectConnectButton.interactable = false;
                };
                WalletConnect.Instance.ConnectedEvent.AddListener(async () =>
                {
                    connecting = false;
                    connected = true;
                    indirectConnectionStatusLabel.text = "Connected";
                    indirectAddressesBox.text = string.Join("\n", await web3IndirectClient.Eth.Accounts.SendRequestAsync());
                    indirectQRImage.sprite = null;
                    indirectDisconnectButton.interactable = true;
                    indirectBalanceOfButton.interactable = true;
                    indirectSendTokensButton.interactable = true;
                });
                WalletConnect.Instance.DisconnectedEvent.AddListener((_) =>
                {
                    connected = false;
                    indirectConnectionStatusLabel.text = "Not Connected";
                    indirectConnectButton.interactable = true;
                    indirectDisconnectButton.interactable = false;
                    indirectBalanceOfButton.interactable = false;
                    indirectSendTokensButton.interactable = false;
                });
            }
        }
    }
}
