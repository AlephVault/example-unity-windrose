using UnityEngine;
using Mirror;
using NetRose.Behaviours.Entities.Objects;
using NetRose.Behaviours.UI.Inventory;
using GameMeanMachine.Unity.BackPack.Authoring.Behaviours.Inventory.Standard;
using GameMeanMachine.Unity.BackPack.Authoring.Behaviours.UI.Inventory.Basic;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Objects.Bags;
using NetRose.Behaviours.UI;
using NetRose.Behaviours.Sessions;

namespace NetworkedSamples
{
    namespace Behaviours
    {
        using Sessions;
        using Sessions.Messages;

        [RequireComponent(typeof(NetworkedMapObjectFollower))]
        [RequireComponent(typeof(NetworkedStandardInventoryView))]
        public class SamplePlayer : StandardPlayer<int, SampleDatabase.Account, int, string, SampleDatabase.Character, SampleChooseCharacter, SampleUsingCharacter, SampleInvalidCharacterID, SampleCharacterDoesNotExist>
        {
            // The integrated single inventory view.
            private NetworkedStandardInventoryView inventoryView;

            // The inventory of the followed object (if any).
            public StandardInventory Inventory { get; private set; }

            // The bag of the followed object (if any).
            public StandardBag Bag { get; private set; }

            protected override void Awake()
            {
                base.Awake();
                inventoryView = GetComponent<NetworkedStandardInventoryView>();
                follower.onTargetChanged.AddListener(delegate (NetworkedMapObject oldObject, NetworkedMapObject newObject)
                {
                    if (oldObject)
                    {
                        oldObject.MapObject.onMovementStarted.RemoveListener(OnMovementStarted);
                        Inventory = oldObject.GetComponent<StandardInventory>();
                        if (Inventory)
                        {
                            Inventory.RenderingStrategy.Broadcaster.RemoveListener(inventoryView);
                        }
                    }
                    if (newObject)
                    {
                        newObject.MapObject.onMovementStarted.AddListener(OnMovementStarted);
                        Inventory = newObject.GetComponent<StandardInventory>();
                        if (Inventory)
                        {
                            Inventory.RenderingStrategy.Broadcaster.AddListener(inventoryView);
                        }
                    }

                    if (Inventory)
                    {
                        Bag = Inventory.GetComponent<StandardBag>();
                    }
                    else
                    {
                        Bag = null;
                    }
                });
            }

            private void OnMovementStarted(GameMeanMachine.Unity.WindRose.Types.Direction direction)
            {
                CurrentCharacter.MapObject.Orientation = direction;
            }

            /// <summary>
            ///   On clients, a camera will be searched, and also a
            ///     <see cref="BasicStandardInventoryView"/> will be
            ///     searched, via the tag: "Inventory".
            /// </summary>
            protected override void OnStartedBeingLocalPlayer()
            {
                base.OnStartedBeingLocalPlayer();
                GameObject basicViewObj = GameObject.FindGameObjectWithTag("Inventory");
                if (basicViewObj)
                {
                    BasicStandardInventoryView basicView = basicViewObj.GetComponent<BasicStandardInventoryView>();
                    if (basicView) inventoryView.Broadcaster.AddListener(basicView);
                }
            }

            /// <summary>
            ///   On clients, both camera and local inventory view
            ///     will be released..
            /// </summary>
            protected override void OnStoppedBeingLocalPlayer()
            {
                base.OnStoppedBeingLocalPlayer();
                GameObject basicViewObj = GameObject.FindGameObjectWithTag("Inventory");
                if (basicViewObj)
                {
                    BasicStandardInventoryView basicView = basicViewObj.GetComponent<BasicStandardInventoryView>();
                    if (basicView) inventoryView.Broadcaster.RemoveListener(basicView);
                }
            }

            protected override NetworkedMapObject InstantiateCharacter()
            {
                throw new System.NotImplementedException();
            }

            protected override void DisposeCharacter(NetworkedMapObject character)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
