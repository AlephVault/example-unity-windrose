using System;
using UnityEngine;
using Mirror;

namespace NetRose
{
    namespace Behaviours
    {
        using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.World;
        using GameMeanMachine.Unity.WindRose.BackPack.Authoring.Behaviours.World.Layers.Drop;
        using GameMeanMachine.Unity.BackPack.Authoring.ScriptableObjects.Inventory.Items;
        using Types.Inventory;

        namespace World
        {
            namespace Layers
            {
                namespace Drop
                {
                    /// <summary>
                    ///   Networked inventory drop layer rendering listeners are the
                    ///     network-side versions of the <see cref="InventoryDropLayerRenderingListener" />
                    ///     and, on client-side, they connect to them in order to get
                    ///     them refreshed with the synchronized data of the (x, y, item)
                    ///     to (item, quantity) of the stacks.
                    /// </summary>
                    [RequireComponent(typeof(InventoryDropLayerRenderingListener))]
                    [RequireComponent(typeof(InventoryMapSizedPositioningManagementStrategy))]
                    public class NetworkedInventoryDropLayerRenderingListener : NetworkBehaviour, InventoryDropLayerRenderingManagementStrategy.RenderingListener
                    {
                        /// <summary>
                        ///   The drop position is a combination of the (x, y)
                        ///     position in the map, and the depth/index position
                        ///     inside that (x, y) position.
                        /// </summary>
                        [Serializable]
                        public class DropPosition
                        {
                            public Vector2Int Coordinates;
                            public int Depth;

                            public DropPosition(Vector2Int coordinates, int depth)
                            {
                                Coordinates = coordinates;
                                Depth = depth;
                            }

                            public DropPosition() {}

                            public override bool Equals(object obj)
                            {
                                if (obj is DropPosition)
                                {
                                    DropPosition other = (DropPosition)obj;
                                    if (other.Coordinates.Equals(Coordinates) && other.Depth == Depth)
                                    {
                                        return true;
                                    }
                                }
                                return false;
                            }

                            public override int GetHashCode()
                            {
                                return new Tuple<Vector2Int, int>(Coordinates, Depth).GetHashCode();
                            }
                        }

                        /// <summary>
                        ///   Holds the data in the stack to pass it via network,
                        ///     when no usage state is needed.
                        /// </summary>
                        [Serializable]
                        public class StackData
                        {
                            public Item Item;
                            public NetworkedInventoryQuantities.Quantity Quantity;

                            public StackData(Item item, NetworkedInventoryQuantities.Quantity quantity)
                            {
                                Item = item;
                                Quantity = quantity;
                            }

                            public StackData() {}
                        }

                        /// <summary>
                        ///   This class keeps a track of the whole data to be
                        ///     synchronized via network related to the drop layer
                        ///     (a map layout: (x, y)-index containers, and indexed
                        ///     positions in an infinite layout).
                        /// </summary>
                        public class SyncStandardInventory : SyncDictionary<DropPosition, StackData> { }

                        // The inventory to synchronize.
                        private SyncStandardInventory drop = new SyncStandardInventory();

                        // We are completely sure we have a PositioningStrategy in the underlying object
                        private InventoryMapSizedPositioningManagementStrategy positioningStrategy;

                        // This is the linked listener, on the client side. It will be the target of
                        // the forwarded calls.
                        private InventoryDropLayerRenderingListener clientTarget;

                        protected void Awake()
                        {
                            positioningStrategy = GetComponent<InventoryMapSizedPositioningManagementStrategy>();
                            try
                            {
                                Map map = GetComponent<DropLayer>().Map;
                            }
                            catch (NullReferenceException)
                            {
                                Debug.LogError("This rendering listener must be bound to an object being also a DropLayer");
                                Destroy(gameObject);
                            }

                            if (isClient)
                            {
                                clientTarget = GetComponent<InventoryDropLayerRenderingListener>();
                                if (clientTarget)
                                {
                                    drop.Callback += Drop_Callback;
                                }
                            }
                        }

                        private void Drop_Callback(SyncIDictionary<DropPosition, StackData>.Operation op, DropPosition key, StackData item)
                        {
                            switch (op)
                            {
                                case SyncIDictionary<DropPosition, StackData>.Operation.OP_CLEAR:
                                    clientTarget.Clear();
                                    break;
                                case SyncIDictionary<DropPosition, StackData>.Operation.OP_ADD:
                                case SyncIDictionary<DropPosition, StackData>.Operation.OP_SET:
                                    clientTarget.UpdateStack(key.Coordinates, key.Depth, item.Item, item.Quantity.Raw);
                                    break;
                                case SyncIDictionary<DropPosition, StackData>.Operation.OP_REMOVE:
                                    clientTarget.RemoveStack(key.Coordinates, key.Depth);
                                    break;
                            }
                        }

                        /// <summary>
                        ///   <para>
                        ///     Refreshing the stack passes the data through the network, and the client
                        ///       side will process the data and reflect it appropriately.
                        ///   </para>
                        /// </summary>
                        /// <param name="containerPosition">The (x, y) in-map position</param>
                        /// <param name="stackPosition">The in-place index</param>
                        /// <param name="item">The item to render</param>
                        /// <param name="quantity">The quantity to render</param>
                        public void UpdateStack(Vector2Int containerPosition, int stackPosition, Item item, object quantity)
                        {
                            drop[new DropPosition(containerPosition, stackPosition)] = new StackData(item, new NetworkedInventoryQuantities.Quantity(quantity));
                        }

                        /// <summary>
                        ///   <para>
                        ///     Removing a stack passes the message through the network, and the client
                        ///       side will process the data and reflect it appropriately.
                        ///   </para>
                        /// </summary>
                        /// <param name="containerPosition">The (x, y) in-map position</param>
                        /// <param name="stackPosition">The in-place index</param>
                        public void RemoveStack(Vector2Int containerPosition, int stackPosition)
                        {
                            drop.Remove(new DropPosition(containerPosition, stackPosition));
                        }

                        /// <summary>
                        ///   Clearing all the containers passes the message through the network, and
                        ///     the client side will process the data and reflect it appropriately.
                        /// </summary>
                        public void Clear()
                        {
                            drop.Clear();
                        }
                    }
                }
            }
        }
    }
}
