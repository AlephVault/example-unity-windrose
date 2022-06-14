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
            private Button eventsClearButton;
            private InputField eventsBox;

            private async void LaunchEventRetrievalLoop()
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

            private void AwakeEventsPanel()
            {
                // Links all the relevant UI elements
                // appropriately to refresh the events.
                eventsBox = transform.Find("eventsPanel/eventsBox").GetComponent<InputField>();
                eventsClearButton = transform.Find("eventsPanel/eventsClearButton").GetComponent<Button>();
            }

            private void StartEventsPanel()
            {
                // Initializes the events-fetching routine
                // by linking the appropriate UI elements.
                // Also, listens for the click events in
                // the "Clear" button.
                //
                // Requires the clients to be initialized.
                // In particular, requires the direct
                // client to be initialized.
                eventsClearButton.onClick.AddListener(() =>
                {
                    eventsBox.text = "";
                });
                LaunchEventRetrievalLoop();
            }
        }
    }
}
