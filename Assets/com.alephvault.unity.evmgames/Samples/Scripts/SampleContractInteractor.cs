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
            
            // Panes and tab buttons go here.
            
            private Button pkWalletButton;
            private Button connectedWalletButton;
            private Button eventsButton;
            private Transform pkWalletPanel;
            private Transform connectedWalletPanel;
            private Transform eventsPanel;
            
            // Core variables go here.
            
            // Now, per-pane variables go here.
            
            // Events pane.

            private Button eventsClearButton;
            private InputField eventsBox;
            
            private void TogglePanel(Transform tf, bool show)
            {
                tf.localScale = show ? Vector3.one : Vector3.zero;
            }
            
            //////////////////////////////////
            // Methods to invoke
            //////////////////////////////////
            
            
            //////////////////////////////////
            // Event initializers
            //////////////////////////////////

            private async void CreateEventFiltering()
            {
                Event<Erc20TransferEvent> transferEvent = web3DirectClient.Eth.GetEvent<Erc20TransferEvent>(ContractAddress);
                NewFilterInput filterInput = transferEvent.CreateFilterInput();
                HexBigInteger filterId = await transferEvent.CreateFilterAsync(filterInput);
                while (gameObject)
                {
                    List<EventLog<Erc20TransferEvent>> events = await transferEvent.GetFilterChangesAsync(filterId);
                    foreach (EventLog<Erc20TransferEvent> @event in events)
                    {
                        if (@event.Log.Type == "mined")
                        {
                            string eventLine = $"\n{@event.Log.BlockNumber} - " +
                                               $"{@event.Event.From}->{@event.Event.To} : {@event.Event.Value}";
                            if (string.IsNullOrEmpty(eventsBox.text))
                            {
                                eventsBox.text = eventLine;
                            }
                            else
                            {
                                eventsBox.text += $"\n{eventLine}";
                            }
                        }
                    }

                    float time = 0;
                    while (time < 5f)
                    {
                        await Tasks.Blink();
                        time += Time.deltaTime;
                    }
                }
            }
            
            private void Awake()
            {
                pkWalletButton = transform.Find("pkWalletButton").GetComponent<Button>();
                connectedWalletButton = transform.Find("connectedWalletButton").GetComponent<Button>();
                eventsButton = transform.Find("eventsButton").GetComponent<Button>();
                pkWalletPanel = transform.Find("pkWalletPanel");
                connectedWalletPanel = transform.Find("connectedWalletPanel");
                eventsPanel = transform.Find("eventsPanel");
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
                
                
                
                // Core variables initialization goes here.

                // This one is the server client. Will also be used for events.
                
                
                // Now, per-pane settings go here.

                // 1. Events pane.

                eventsBox = transform.Find("eventsPanel/eventsBox").GetComponent<InputField>();
                eventsClearButton = transform.Find("eventsPanel/eventsClearButton").GetComponent<Button>();
                Debug.Log(eventsBox);
                Debug.Log(eventsClearButton);
                eventsClearButton.onClick.AddListener(() =>
                {
                    eventsBox.text = "";
                });
                CreateEventFiltering();
                
                // 2. Server pane.
                // 3. Client pane.
                
            }
        }
    }
}
