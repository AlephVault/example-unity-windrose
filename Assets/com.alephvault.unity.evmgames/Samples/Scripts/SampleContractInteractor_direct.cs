using UnityEngine.UI;


namespace AlephVault.Unity.EVMGames
{
    namespace Samples
    {
        public partial class SampleContractInteractor
        {
            private Text currentPrivateKey;
            private InputField directAddressesBox;

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
                directAddressesBox = transform.Find("pkWalletPanel/addressesBox").GetComponent<InputField>();

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
                directAddressesBox.text = string.Join("\n", await web3DirectClient.Eth.Accounts.SendRequestAsync());
                currentPrivateKey.text = privateKey;
                directBalanceOfButton.onClick.AddListener( () =>
                {
                    DoBalanceOf(web3DirectClient, directBalanceOfInput, directBalanceOfResult);
                });
                directSendTokensButton.onClick.AddListener(() =>
                {
                    DoTransfer(web3DirectClient, directSendTokensToInput, directSendTokensAmountInput, directSendTokensResult);
                });
            }
        }
    }
}
