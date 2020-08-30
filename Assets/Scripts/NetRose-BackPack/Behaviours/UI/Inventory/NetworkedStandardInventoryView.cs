using BackPack.Behaviours.Inventory.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Mirror;

namespace NetRose
{
    namespace Behaviours
    {
        namespace UI
        {
            namespace Inventory
            {
                using Types.Inventory;
                using BackPack.ScriptableObjects.Inventory.Items;
                using BackPack.Behaviours.UI.Inventory;
                using ScriptableObjects.Inventory.Items;

                /// <summary>
                ///   A networked standard inventory view typically links with a
                ///     <see cref="StandardInventoryView"/> and is synchronized
                ///     through network messages.
                /// </summary>
                class NetworkedStandardInventoryView : NetworkBehaviour, InventoryStandardRenderingManagementStrategy.RenderingListener
                {
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

                        public StackData() { }
                    }

                    /// <summary>
                    ///   This class keeps a track of the whole data to be
                    ///     synchronized via network related to the inventory
                    ///     (a standard layout: one container, and indexed
                    ///     positions - either finite or infinite).
                    /// </summary>
                    public class SyncStandardInventory : SyncDictionary<int, StackData> { }

                    // The inventory to synchronize.
                    private SyncStandardInventory inventory = new SyncStandardInventory();

                    public void Clear()
                    {
                        if (isServer)
                        {
                            inventory.Clear();
                        }
                    }

                    public void Connected()
                    {
                        Clear();
                    }

                    public void Disconnected()
                    {
                        Clear();
                    }

                    public void RemoveStack(int position)
                    {
                        if (isServer)
                        {
                            inventory.Remove(position);
                        }
                    }

                    public void UpdateStack(int stackPosition, Item item, object quantity)
                    {
                        if (isServer)
                        {
                            inventory[stackPosition] = new StackData(item, new NetworkedInventoryQuantities.Quantity(quantity));
                        }
                    }
                }
            }
        }
    }
}
