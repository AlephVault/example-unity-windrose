using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Entities.Objects
        {
            namespace Bags
            {
                using Types.Inventory.Stacks;
                using Inventory;
                using Inventory.ManagementStrategies.SpatialStrategies;
                using System;
                using System.Linq;
                using Support.Types;
                using World.Layers.Drop;

                /// <summary>
                ///   <para>
                ///     Simple bags are intended to be used as portable bags on objects and
                ///       they will handle just ONE container, but with convenience methods
                ///       to abstract the user regarding the inners of the single-container
                ///       positioning strategy. Aside from that, methods to drop and pick
                ///       an object to/from the <see cref="DropLayer"/>.
                ///   </para>
                ///   <para>
                ///     They are tightly related to <see cref="InventoryManagementStrategyHolder"/>
                ///       and <see cref="InventorySimpleBagRenderingManagementStrategy"/>.
                ///   </para>
                /// </summary>
                [RequireComponent(typeof(Objects.MapObject))]
                [RequireComponent(typeof(InventorySinglePositioningManagementStrategy))]
                [RequireComponent(typeof(InventorySimpleSpatialManagementStrategy))]
                [RequireComponent(typeof(InventoryManagementStrategyHolder))]
                [RequireComponent(typeof(InventorySimpleBagRenderingManagementStrategy))]
                public class SimpleBag : MonoBehaviour
                {
                    private InventoryManagementStrategyHolder inventoryHolder;
                    private Objects.MapObject mapObject;

                    /**
                     * Awake/Start pre-register the renderers (if they are set).
                     */

                    void Awake()
                    {
                        inventoryHolder = GetComponent<InventoryManagementStrategyHolder>();
                        mapObject = GetComponent<Objects.MapObject>();
                    }

                    /**
                     * Proxy calls to inventory holder methods.
                     */

                    /// <summary>
                    ///   Convenience method. See <see cref="InventoryManagementStrategyHolder.StackPairs(object, bool)"/>.
                    /// </summary>
                    public IEnumerable<Tuple<int, Stack>> StackPairs(bool reverse = false)
                    {
                        return from tuple in inventoryHolder.StackPairs(Position.Instance, reverse) select new Tuple<int, Stack>((int)tuple.First, tuple.Second);
                    }

                    /// <summary>
                    ///   Convenience method. See <see cref="InventoryManagementStrategyHolder.Find(object, object)"/>.
                    /// </summary>
                    public Stack Find(int position)
                    {
                        return inventoryHolder.Find(Position.Instance, position);
                    }

                    /// <summary>
                    ///   Convenience method. See <see cref="InventoryManagementStrategyHolder.FindAll(object, Func{Tuple{object, Stack}, bool}, bool)"/>.
                    /// </summary>
                    public IEnumerable<Stack> FindAll(Func<Tuple<int, Stack>, bool> predicate, bool reverse = false)
                    {
                        return inventoryHolder.FindAll(Position.Instance, delegate (Tuple<object, Stack> tuple) { return predicate(new Tuple<int, Stack>((int)tuple.First, tuple.Second)); }, reverse);
                    }

                    /// <summary>
                    ///   Convenience method. See <see cref="InventoryManagementStrategyHolder.FindAll(object, ScriptableObjects.Inventory.Items.Item, bool)"/>.
                    /// </summary>
                    public IEnumerable<Stack> FindAll(ScriptableObjects.Inventory.Items.Item item, bool reverse = false)
                    {
                        return inventoryHolder.FindAll(Position.Instance, item, reverse);
                    }

                    /// <summary>
                    ///   Convenience method. See <see cref="InventoryManagementStrategyHolder.First(object)"/>.
                    /// </summary>
                    public Stack First()
                    {
                        return inventoryHolder.First(Position.Instance);
                    }

                    /// <summary>
                    ///   Convenience method. See <see cref="InventoryManagementStrategyHolder.Last(object)"/>.
                    /// </summary>
                    public Stack Last()
                    {
                        return inventoryHolder.Last(Position.Instance);
                    }

                    /// <summary>
                    ///   Convenience method. See <see cref="InventoryManagementStrategyHolder.FindOne(object, Func{Tuple{object, Stack}, bool}, bool)"/>.
                    /// </summary>
                    public Stack FindOne(Func<Tuple<int, Stack>, bool> predicate, bool reverse = false)
                    {
                        return inventoryHolder.FindOne(Position.Instance, delegate (Tuple<object, Stack> tuple) { return predicate(new Tuple<int, Stack>((int)tuple.First, tuple.Second)); }, reverse);
                    }

                    /// <summary>
                    ///   Convenience method. See <see cref="InventoryManagementStrategyHolder.FindOne(object, ScriptableObjects.Inventory.Items.Item, bool)"/>.
                    /// </summary>
                    public Stack FindOne(ScriptableObjects.Inventory.Items.Item item, bool reverse = false)
                    {
                        return inventoryHolder.FindOne(Position.Instance, item, reverse);
                    }

                    /// <summary>
                    ///   Convenience method. See <see cref="InventoryManagementStrategyHolder.Put(object, object, Stack, out object, bool?)"/>.
                    /// </summary>
                    public bool Put(int? position, Stack stack, out int? finalPosition, bool? optimalPutOnNullPosition = null)
                    {
                        object finalOPosition;
                        bool result = inventoryHolder.Put(Position.Instance, position, stack, out finalOPosition, optimalPutOnNullPosition);
                        finalPosition = finalOPosition == null ? null : (int?)finalOPosition;
                        return result;
                    }

                    /// <summary>
                    ///   Convenience method. See <see cref="InventoryManagementStrategyHolder.Remove(object, object)"/>.
                    /// </summary>
                    public bool Remove(int position)
                    {
                        return inventoryHolder.Remove(Position.Instance, position);
                    }

                    /// <summary>
                    ///   Convenience method. See <see cref="InventoryManagementStrategyHolder.Merge(object, object, object)"/>.
                    /// </summary>
                    public bool Merge(int? destinationPosition, int sourcePosition)
                    {
                        return inventoryHolder.Merge(Position.Instance, destinationPosition, sourcePosition);
                    }

                    // The other version of `Merge` has little use here.

                    /// <summary>
                    ///   Convenience method. See <see cref="InventoryManagementStrategyHolder.Take(object, object, object, bool)"/>.
                    /// </summary>
                    public Stack Take(int position, object quantity, bool disallowEmpty)
                    {
                        return inventoryHolder.Take(Position.Instance, position, quantity, disallowEmpty);
                    }

                    /// <summary>
                    ///   Convenience method. See <see cref="InventoryManagementStrategyHolder.Split(object, object, object, object, object, out object)"/>.
                    /// </summary>
                    public bool Split(int sourcePosition, object quantity,
                                      int newPosition, int? finalNewPosition)
                    {
                        object finalNewOPosition;
                        bool result = inventoryHolder.Split(Position.Instance, sourcePosition, quantity,
                                                            Position.Instance, newPosition, out finalNewOPosition);
                        finalNewPosition = finalNewOPosition == null ? null : (int?)finalNewOPosition;
                        return result;
                    }

                    /// <summary>
                    ///   Convenience method. See <see cref="InventoryManagementStrategyHolder.Use(object, object)"/>.
                    /// </summary>
                    public bool Use(int sourcePosition)
                    {
                        return inventoryHolder.Use(Position.Instance, sourcePosition);
                    }

                    /// <summary>
                    ///   Convenience method. See <see cref="InventoryManagementStrategyHolder.Use(object, object, object)"/>.
                    /// </summary>
                    public bool Use(int sourcePosition, object argument)
                    {
                        return inventoryHolder.Use(Position.Instance, sourcePosition, argument);
                    }

                    /// <summary>
                    ///   Convenience method. See <see cref="InventoryManagementStrategyHolder.Clear"/>.
                    /// </summary>
                    public void Clear()
                    {
                        inventoryHolder.Clear();
                    }

                    /// <summary>
                    ///   Convenience method. See <see cref="InventoryManagementStrategyHolder.Blink(object)"/>.
                    /// </summary>
                    public void Blink()
                    {
                        inventoryHolder.Blink(Position.Instance);
                    }

                    /// <summary>
                    ///   Convenience method. See <see cref="InventoryManagementStrategyHolder.Blink(object, object)"/>.
                    /// </summary>
                    public void Blink(int position)
                    {
                        inventoryHolder.Blink(Position.Instance, position);
                    }

                    /// <summary>
                    ///   Convenience method. See <see cref="InventoryManagementStrategyHolder.Import(Types.Inventory.SerializedInventory)"/>.
                    /// </summary>
                    public void Import(Types.Inventory.SerializedInventory serializedInventory)
                    {
                        inventoryHolder.Import(serializedInventory);
                    }

                    /// <summary>
                    ///   Convenience method. See <see cref="InventoryManagementStrategyHolder.Export"/>.
                    /// </summary>
                    public Types.Inventory.SerializedInventory Export()
                    {
                        return inventoryHolder.Export();
                    }

                    // TODO Add/Remove listener will be different here!

                    /**
                     * These are convenience methods to interact, in particular, with a drop layer.
                     */
                    
                    private DropLayer GetDropLayer()
                    {
                        if (mapObject.ParentMap == null)
                        {
                            return null;
                        }

                        return mapObject.ParentMap.DropLayer;
                    }

                    /// <summary>
                    ///   Drops an element in certain bag's position, into the current object's in-map
                    ///     position.
                    /// </summary>
                    /// <param name="position">The position of the stack to drop</param>
                    /// <param name="quantity">How much to take & drop from the stack</param>
                    /// <returns>Whether it could drop that quantity in the underlying drop layer, or not</returns>
                    /// <remarks>
                    ///   The drop position on which to place the dropped object will be the lower-left
                    ///     corner of the simple bag holder - for this reason this method is better
                    ///     suited for 1x1 objects.
                    /// </remarks>
                    public bool Drop(int position, object quantity = null)
                    {
                        DropLayer dropLayer = GetDropLayer();
                        if (dropLayer == null)
                        {
                            return false;
                        }

                        Stack found = Find(position);
                        if (found != null)
                        {
                            Stack taken = Take(position, quantity, false);
                            if (taken != null)
                            {
                                object finalStackPosition;
                                // This call will NEVER fail: drop layers have infinite length.
                                return dropLayer.Push(new Vector2Int((int)mapObject.X, (int)mapObject.Y), taken, out finalStackPosition);
                            }
                        }

                        return false;
                    }

                    /// <summary>
                    ///   Takes an element from the underlying drop layer, from the position this object
                    ///     is standing at.
                    /// </summary>
                    /// <param name="finalPosition">The returned final position of the just-picked object, if any</param>
                    /// <param name="optimalPick">
                    ///   Whether an optimal pick should be performed. 
                    ///   See <see cref="InventoryManagementStrategyHolder.Put(object, object, Stack, out object, bool?)"/> for more details
                    /// </param>
                    /// <returns>Whether it could pick an item from the floor or not</returns>
                    /// <remarks>
                    ///   The drop position from which objects will be picked will be the lower-left
                    ///     corner of the simple bag holder - for this reason this method is better
                    ///     suited for 1x1 objects.
                    /// </remarks>
                    public bool Pick(out int? finalPosition, bool? optimalPick = null)
                    {
                        DropLayer dropLayer = GetDropLayer();
                        if (dropLayer == null)
                        {
                            finalPosition = null;
                            return false;
                        }

                        Vector2Int containerPosition = new Vector2Int((int)mapObject.X, (int)mapObject.Y);
                        Stack found = dropLayer.Last(containerPosition);
                        if (found != null)
                        {
                            bool result = Put(null, found.Clone(), out finalPosition, optimalPick);
                            if (result)
                            {
                                dropLayer.Remove(containerPosition, (int)found.QualifiedPosition.First);
                            }

                            return result;
                        }

                        finalPosition = null;
                        return false;
                    }
                }
            }
        }
    }
}
