using UnityEngine;
using BackPack.Behaviours.Inventory.Standard;

namespace NetRose
{
    namespace Behaviours
    {
        namespace UI
        {
            namespace Inventory
            {
                /// <summary>
                ///   Links an inventory and its related view through an
                ///     exposed property to select the inventory. Once
                ///     selected, the view will also be linked to it.
                ///     Also, on component startup, if an inventory is
                ///     selected, it will also link the view to it. This
                ///     method is the networked counterpart of the
                ///     Basic Standard Inventory View in the BackPack
                ///     integration to WindRose.
                /// </summary>
                [RequireComponent(typeof(NetworkedStandardInventoryView))]
                public class NetworkedStandardInventoryLink : MonoBehaviour
                {
                    // The view component to perform the link.
                    private NetworkedStandardInventoryView inventoryView;

                    /// <summary>
                    ///   The inventory this control will be bound to on start.
                    /// </summary>
                    [SerializeField]
                    private StandardInventory inventory;

                    private void Awake()
                    {
                        inventoryView = GetComponent<NetworkedStandardInventoryView>();
                    }

                    private void Start()
                    {
                        if (inventory) inventory.GetComponent<InventoryStandardRenderingManagementStrategy>().Broadcaster.AddListener(inventoryView);
                    }

                    /// <summary>
                    ///   Sets or gets the current inventory this link is bound to. On change,
                    ///     the former inventory will not be watched anymore by this link, and
                    ///     the new one will start to be watched by this link.
                    /// </summary>
                    public StandardInventory Inventory
                    {
                        get
                        {
                            return inventory;
                        }
                        set
                        {
                            if (inventory) inventory.GetComponent<InventoryStandardRenderingManagementStrategy>().Broadcaster.RemoveListener(inventoryView);
                            inventory = value;
                            if (inventory) inventory.GetComponent<InventoryStandardRenderingManagementStrategy>().Broadcaster.AddListener(inventoryView);
                        }
                    }
                }
            }
        }
    }
}
