﻿using System.Collections;
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
                using World.Layers.Drop;

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
                    private Positionable positionable;

                    /**
                     * Awake/Start pre-register the renderers (if they are set).
                     */

                    void Awake()
                    {
                        inventoryHolder = GetComponent<InventoryManagementStrategyHolder>();
                        positionable = GetComponent<Positionable>();
                    }

                    /**
                     * Proxy calls to inventory holder methods.
                     */

                    public IEnumerable<Tuple<int, Stack>> StackPairs(bool reverse = false)
                    {
                        return from tuple in inventoryHolder.StackPairs(Position.Instance, reverse) select new Tuple<int, Stack>((int)tuple.First, tuple.Second);
                    }

                    public Stack Find(int position)
                    {
                        return inventoryHolder.Find(Position.Instance, position);
                    }

                    public IEnumerable<Stack> FindAll(Func<Tuple<int, Stack>, bool> predicate, bool reverse = false)
                    {
                        return inventoryHolder.FindAll(Position.Instance, delegate (Tuple<object, Stack> tuple) { return predicate(new Tuple<int, Stack>((int)tuple.First, tuple.Second)); }, reverse);
                    }

                    public IEnumerable<Stack> FindAll(ScriptableObjects.Inventory.Items.Item item, bool reverse = false)
                    {
                        return inventoryHolder.FindAll(Position.Instance, item, reverse);
                    }

                    public Stack First()
                    {
                        return inventoryHolder.First(Position.Instance);
                    }

                    public Stack Last()
                    {
                        return inventoryHolder.Last(Position.Instance);
                    }

                    public Stack FindOne(Func<Tuple<int, Stack>, bool> predicate, bool reverse = false)
                    {
                        return inventoryHolder.FindOne(Position.Instance, delegate (Tuple<object, Stack> tuple) { return predicate(new Tuple<int, Stack>((int)tuple.First, tuple.Second)); }, reverse);
                    }

                    public Stack FindOne(ScriptableObjects.Inventory.Items.Item item, bool reverse = false)
                    {
                        return inventoryHolder.FindOne(Position.Instance, item, reverse);
                    }

                    public bool Put(int? position, Stack stack, out int? finalPosition, bool? optimalPutOnNullPosition = null)
                    {
                        object finalOPosition;
                        bool result = inventoryHolder.Put(Position.Instance, position, stack, out finalOPosition, optimalPutOnNullPosition);
                        finalPosition = finalOPosition == null ? null : (int?)finalOPosition;
                        return result;
                    }

                    public bool Remove(int position)
                    {
                        return inventoryHolder.Remove(Position.Instance, position);
                    }

                    public bool Merge(int? destinationPosition, int sourcePosition)
                    {
                        return inventoryHolder.Merge(Position.Instance, destinationPosition, sourcePosition);
                    }

                    // The other version of `Merge` has little use here.

                    public Stack Take(int position, object quantity, bool disallowEmpty)
                    {
                        return inventoryHolder.Take(Position.Instance, position, quantity, disallowEmpty);
                    }

                    public bool Split(int sourcePosition, object quantity,
                                      int newPosition, int? finalNewPosition)
                    {
                        object finalNewOPosition;
                        bool result = inventoryHolder.Split(Position.Instance, sourcePosition, quantity,
                                                            Position.Instance, newPosition, out finalNewOPosition);
                        finalNewPosition = finalNewOPosition == null ? null : (int?)finalNewOPosition;
                        return result;
                    }

                    public bool Use(int sourcePosition)
                    {
                        return inventoryHolder.Use(Position.Instance, sourcePosition);
                    }

                    public bool Use(int sourcePosition, object argument)
                    {
                        return inventoryHolder.Use(Position.Instance, sourcePosition, argument);
                    }

                    public void Clear()
                    {
                        inventoryHolder.Clear();
                    }

                    public void Blink()
                    {
                        inventoryHolder.Blink(Position.Instance);
                    }

                    public void Blink(int position)
                    {
                        inventoryHolder.Blink(Position.Instance, position);
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

                    /**
                     * These are convenience methods to interact, in particular, with a drop layer.
                     */
                    
                    private DropLayer GetDropLayer()
                    {
                        if (positionable.ParentMap == null)
                        {
                            return null;
                        }

                        return positionable.ParentMap.DropLayer;
                    }

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
                                return dropLayer.Push(new Vector2Int((int)positionable.X, (int)positionable.Y), taken, out finalStackPosition);
                            }
                        }

                        return false;
                    }

                    public bool Pick(out int? finalPosition, bool? optimalPick = null)
                    {
                        DropLayer dropLayer = GetDropLayer();
                        if (dropLayer == null)
                        {
                            finalPosition = null;
                            return false;
                        }

                        Vector2Int containerPosition = new Vector2Int((int)positionable.X, (int)positionable.Y);
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
