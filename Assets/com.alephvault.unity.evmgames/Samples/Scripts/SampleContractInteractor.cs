using System;
using AlephVault.Unity.EVMGames.WalletConnectSharp.Unity;
using UnityEngine;
using UnityEngine.UI;


namespace AlephVault.Unity.EVMGames
{
    namespace Samples
    {
        public class SampleContractInteractor : MonoBehaviour
        {
            private Button pkWalletButton;
            private Button connectedWalletButton;
            private Button eventsButton;
            private Transform pkWalletPanel;
            private Transform connectedWalletPanel;
            private Transform eventsPanel;
            private WalletConnect walletConnect;
            private InputField eventsBox;
            
            // Now, per-pane variables go here.

            private Button eventsClearButton;

            private void TogglePanel(Transform tf, bool show)
            {
                tf.localScale = show ? Vector3.one : Vector3.zero;
            }
            
            private void Awake()
            {
                pkWalletButton = transform.Find("pkWalletButton").GetComponent<Button>();
                connectedWalletButton = transform.Find("connectedWalletButton").GetComponent<Button>();
                eventsButton = transform.Find("eventsButton").GetComponent<Button>();
                pkWalletPanel = transform.Find("pkWalletPanel");
                connectedWalletPanel = transform.Find("connectedWalletPanel");
                eventsPanel = transform.Find("eventsPanel");
                walletConnect = transform.Find("WalletConnect").GetComponent<WalletConnect>();
                TogglePanel(pkWalletPanel, false);
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
                
                // Now, per-pane settings go here.

                eventsBox = transform.Find("eventsPanel/eventsBox").GetComponent<InputField>();
                eventsClearButton = transform.Find("eventsPanel/eventsClearButton").GetComponent<Button>();
                Debug.Log(eventsBox);
                Debug.Log(eventsClearButton);
                eventsClearButton.onClick.AddListener(() =>
                {
                    eventsBox.text = "";
                });
            }
        }
    }
}
