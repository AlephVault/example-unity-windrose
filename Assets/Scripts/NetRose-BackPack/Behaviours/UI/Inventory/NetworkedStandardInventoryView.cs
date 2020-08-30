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
                ///   <para>
                ///     A networked standard inventory view typically links with a
                ///       <see cref="StandardInventoryView"/> and is synchronized
                ///       through network messages.
                ///   </para>
                ///   <para>
                ///     When this component starts on server, it just cares about
                ///       sending stuff via networking. On client, it cares about
                ///       attending synchronization callbacks (in the inventory
                ///       data) and linking the synchronization operations with
                ///       calls to a newly related <see cref="StandardInventoryView"/>
                ///       element to actually sync the data.
                ///   </para>
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

                    // On the client side, the target of the syncronization.
                    private StandardInventoryView clientTarget;

                    private void Awake()
                    {
                        // This behaviour must only synchronize on Owner.
                        syncMode = SyncMode.Owner;
                        // On the client side, a callback must attend the inventory's update,
                        // and the target view must also be recognized.
                        if (isClient)
                        {
                            clientTarget = GetComponent<StandardInventoryView>();
                            if (clientTarget)
                            {
                                inventory.Callback += Inventory_Callback;
                            }
                        }
                    }

                    private void Inventory_Callback(SyncIDictionary<int, StackData>.Operation op, int key, StackData item)
                    {
                        switch(op)
                        {
                            case SyncIDictionary<int, StackData>.Operation.OP_CLEAR:
                                clientTarget.Clear();
                                break;
                            case SyncIDictionary<int, StackData>.Operation.OP_ADD:
                            case SyncIDictionary<int, StackData>.Operation.OP_SET:
                                clientTarget.UpdateStack(key, item.Item, item.Quantity.Raw);
                                break;
                            case SyncIDictionary<int, StackData>.Operation.OP_REMOVE:
                                clientTarget.RemoveStack(key);
                                break;
                        }
                    }

                    /// <summary>
                    ///   Forwards a clear call to the client through the inventory dictionary.
                    /// </summary>
                    public void Clear()
                    {
                        if (isServer)
                        {
                            inventory.Clear();
                        }
                    }

                    /// <summary>
                    ///   On connected, it just clears its content.
                    /// </summary>
                    public void Connected()
                    {
                        Clear();
                    }

                    /// <summary>
                    ///   On disconnected, it just clears its content.
                    /// </summary>
                    public void Disconnected()
                    {
                        Clear();
                    }

                    /// <summary>
                    ///   Forwards a removal call to the client through the inventory dictionary.
                    /// </summary>
                    public void RemoveStack(int position)
                    {
                        if (isServer)
                        {
                            inventory.Remove(position);
                        }
                    }

                    /// <summary>
                    ///   Forwards an update call to the client through the inventory dictionary.
                    /// </summary>
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
