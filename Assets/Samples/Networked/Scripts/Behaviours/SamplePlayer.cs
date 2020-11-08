using UnityEngine;
using Mirror;
using NetRose.Behaviours.Entities.Objects;
using NetRose.Behaviours.UI.Inventory;
using BackPack.Behaviours.Inventory.Standard;
using BackPack.Behaviours.UI.Inventory.Basic;
using WindRose.Behaviours.Entities.Objects.Bags;
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
            private StandardInventory inventory;

            // The bag of the followed object (if any).
            private StandardBag bag;

            protected override void Awake()
            {
                base.Awake();
                inventoryView = GetComponent<NetworkedStandardInventoryView>();
                follower.onTargetChanged.AddListener(delegate (NetworkedMapObject oldObject, NetworkedMapObject newObject)
                {
                    if (oldObject)
                    {
                        oldObject.MapObject.onMovementStarted.RemoveListener(OnMovementStarted);
                        inventory = oldObject.GetComponent<StandardInventory>();
                        if (inventory)
                        {
                            inventory.RenderingStrategy.Broadcaster.RemoveListener(inventoryView);
                        }
                    }
                    if (newObject)
                    {
                        newObject.MapObject.onMovementStarted.AddListener(OnMovementStarted);
                        inventory = newObject.GetComponent<StandardInventory>();
                        if (inventory)
                        {
                            inventory.RenderingStrategy.Broadcaster.AddListener(inventoryView);
                        }
                    }

                    if (inventory)
                    {
                        bag = inventory.GetComponent<StandardBag>();
                    }
                    else
                    {
                        bag = null;
                    }
                });
            }

            private void OnMovementStarted(WindRose.Types.Direction direction)
            {
                currentCharacter.MapObject.Orientation = direction;
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

            [Command]
            public void Pick()
            {
                if (bag) bag.Pick(out _);
            }

            [Command]
            public void Drop(int position)
            {
                if (bag) bag.Drop(position);
            }

            [Command]
            public void Right()
            {
                if (currentCharacter)
                {
                    currentCharacter.MapObject.StartMovement(WindRose.Types.Direction.RIGHT);
                }
            }

            [Command]
            public void Up()
            {
                if (currentCharacter)
                {
                    currentCharacter.MapObject.StartMovement(WindRose.Types.Direction.UP);
                }
            }

            [Command]
            public void Left()
            {
                if (currentCharacter)
                {
                    currentCharacter.MapObject.StartMovement(WindRose.Types.Direction.LEFT);
                }
            }

            [Command]
            public void Down()
            {
                if (currentCharacter)
                {
                    currentCharacter.MapObject.StartMovement(WindRose.Types.Direction.DOWN);
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
