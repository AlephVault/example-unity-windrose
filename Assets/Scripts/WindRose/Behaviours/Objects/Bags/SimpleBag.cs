using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Objects
        {
            namespace Bags
            {
                using Types.Inventory.Stacks;
                using Inventory;
                using Inventory.ManagementStrategies.SpatialStrategies;
                using System;
                using System.Linq;
                using Support.Types;

                [RequireComponent(typeof(Positionable))]
                [RequireComponent(typeof(InventorySinglePositioningManagementStrategy))]
                [RequireComponent(typeof(InventorySimpleSpatialManagementStrategy))]
                [RequireComponent(typeof(InventoryManagementStrategyHolder))]
                [RequireComponent(typeof(InventorySimpleBagRenderingManagementStrategy))]
                public class SimpleBag : MonoBehaviour
                {
                    /**
                     * Simple bags involve simple positioning (finite or infinite),
                     *   simple rendering (which involves knowing the size and also
                     *   image/caption/quantity of a stack), and single positioning
                     *   (which validates only the null position and iterates only
                     *   yielding the null position.
                     * 
                     * There is a difference here with respect to the DropLayer: this
                     *   class will not be the same executing the logic and connecting
                     *   to the renderer, but instead be connected to many renderers.
                     */

                    private InventoryManagementStrategyHolder inventoryHolder;

                    /**
                     * Awake/Start pre-register the renderers (if they are set).
                     */

                    void Awake()
                    {
                        inventoryHolder = GetComponent<InventoryManagementStrategyHolder>();
                    }

                    /**
                     * Proxy calls to inventory holder methods.
                     */

                    public IEnumerable<Tuple<int, Stack>> StackPairs(bool reverse = false)
                    {
                        return from tuple in inventoryHolder.StackPairs(null, reverse) select new Tuple<int, Stack>((int)tuple.First, tuple.Second);
                    }

                    public Stack Find(int stackPosition)
                    {
                        return inventoryHolder.Find(null, stackPosition);
                    }

                    public IEnumerable<Stack> FindAll(Func<Tuple<int, Stack>, bool> predicate, bool reverse = false)
                    {
                        return inventoryHolder.FindAll(null, delegate (Tuple<object, Stack> tuple) { return predicate(new Tuple<int, Stack>((int)tuple.First, tuple.Second)); }, reverse);
                    }

                    public IEnumerable<Stack> FindAll(ScriptableObjects.Inventory.Items.Item item, bool reverse = false)
                    {
                        return inventoryHolder.FindAll(null, item, reverse);
                    }

                    public Stack First()
                    {
                        return inventoryHolder.First(null);
                    }

                    public Stack Last()
                    {
                        return inventoryHolder.Last(null);
                    }

                    public Stack FindOne(Func<Tuple<int, Stack>, bool> predicate, bool reverse = false)
                    {
                        return inventoryHolder.FindOne(null, delegate (Tuple<object, Stack> tuple) { return predicate(new Tuple<int, Stack>((int)tuple.First, tuple.Second)); }, reverse);
                    }

                    public Stack FindOne(ScriptableObjects.Inventory.Items.Item item, bool reverse = false)
                    {
                        return inventoryHolder.FindOne(null, item, reverse);
                    }

                    public bool Put(int stackPosition, Stack stack, int? finalStackPosition, bool? optimalPutOnNullPosition = null)
                    {
                        object finalOStackPosition;
                        bool result = inventoryHolder.Put(null, stackPosition, stack, out finalOStackPosition, optimalPutOnNullPosition);
                        finalStackPosition = finalOStackPosition == null ? null : (int?)finalOStackPosition;
                        return result;
                    }

                    public bool Remove(int stackPosition)
                    {
                        return inventoryHolder.Remove(null, stackPosition);
                    }

                    public bool Merge(int? destinationStackPosition, int sourceStackPosition)
                    {
                        return inventoryHolder.Merge(null, destinationStackPosition, sourceStackPosition);
                    }

                    // The other version of `Merge` has little use here.

                    public Stack Take(int stackPosition, object quantity)
                    {
                        return inventoryHolder.Take(null, stackPosition, quantity);
                    }

                    public bool Split(int sourceStackPosition, object quantity,
                                      int newStackPosition, int? finalNewStackPosition)
                    {
                        object finalNewOStackPosition;
                        bool result = inventoryHolder.Split(null, sourceStackPosition, quantity,
                                                            null, newStackPosition, out finalNewOStackPosition);
                        finalNewStackPosition = finalNewOStackPosition == null ? null : (int?)finalNewOStackPosition;
                        return result;
                    }

                    public bool Use(int sourceStackPosition)
                    {
                        return inventoryHolder.Use(null, sourceStackPosition);
                    }

                    public bool Use(int sourceStackPosition, object argument)
                    {
                        return inventoryHolder.Use(null, sourceStackPosition, argument);
                    }

                    public void Clear()
                    {
                        inventoryHolder.Clear();
                    }

                    public void Blink()
                    {
                        inventoryHolder.Blink(null);
                    }

                    public void Blink(int stackPosition)
                    {
                        inventoryHolder.Blink(null, stackPosition);
                    }

                    public void Import(Types.Inventory.SerializedInventory serializedInventory)
                    {
                        inventoryHolder.Import(serializedInventory);
                    }

                    public Types.Inventory.SerializedInventory Export()
                    {
                        return inventoryHolder.Export();
                    }

                    // TODO Add/Remove listener will be different here!
                }
            }
        }
    }
}
