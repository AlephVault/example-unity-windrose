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
            private Button pkWalletButton;
            private Button connectedWalletButton;
            private Button eventsButton;
            private Transform pkWalletPanel;
            private Transform connectedWalletPanel;
            private Transform eventsPanel;

            private void TogglePanel(Transform tf, bool show)
            {
                // Toggles a tab on or off.
                tf.localScale = show ? Vector3.one : Vector3.zero;
            }

            private void AwakeTabs()
            {
                pkWalletButton = transform.Find("pkWalletButton").GetComponent<Button>();
                connectedWalletButton = transform.Find("connectedWalletButton").GetComponent<Button>();
                eventsButton = transform.Find("eventsButton").GetComponent<Button>();
                pkWalletPanel = transform.Find("pkWalletPanel");
                connectedWalletPanel = transform.Find("connectedWalletPanel");
                eventsPanel = transform.Find("eventsPanel");
            }

            private void StartTabs()
            {
                // Toggles all the panels and attaches events to the tabs.
                // The only panel that is shown is the Direct / Wallet one.
                TogglePanel(pkWalletPanel, true);
                TogglePanel(connectedWalletPanel, false);
                TogglePanel(eventsPanel, false);
                
                pkWalletButton.onClick.AddListener(() =>
                {
                    Debug.Log("pk wallet panel");
                    TogglePanel(pkWalletPanel, true);
                    TogglePanel(connectedWalletPanel, false);
                    TogglePanel(eventsPanel, false);
                });
                connectedWalletButton.onClick.AddListener(() =>
                {
                    Debug.Log("connected wallet panel");
                    TogglePanel(pkWalletPanel, false);
                    TogglePanel(connectedWalletPanel, true);
                    TogglePanel(eventsPanel, false);
                });
                eventsButton.onClick.AddListener(() =>
                {
                    Debug.Log("events panel");
                    TogglePanel(pkWalletPanel, false);
                    TogglePanel(connectedWalletPanel, false);
                    TogglePanel(eventsPanel, true);
                });
            }
        }
    }
}
