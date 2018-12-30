using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace World
        {
            namespace Layers
            {
                namespace Drop
                {
                    using Types.Inventory.Stacks;
                    using Inventory;
                    using Inventory.ManagementStrategies.RenderingStrategies;
                    using Inventory.ManagementStrategies.SpatialStrategies;
                    using Support.Types;
                    using System.Linq;

                    [RequireComponent(typeof(InventoryMapSizedPositioningManagementStrategy))]
                    [RequireComponent(typeof(InventorySimpleRenderingManagementStrategy))]
                    [RequireComponent(typeof(InventoryInfiniteSimpleSpatialManagementStrategy))]
                    [RequireComponent(typeof(InventoryManagementStrategyHolder))]
                    [RequireComponent(typeof(DropLayerInventoryRenderer))]
                    public class DropLayer : MapLayer
                    {
                        /**
                         * TODO this class will render the drop layer. This class, however, is also the listener
                         *   for the renderer.
                         * 
                         * While you can directly add/remove items using the holder, it is better if you just
                         *   push / pop the items in the floor (we will treat the items as a "stack" of stacks).
                         * 
                         * We could do it in a different way, but perhaps that odd container management would be
                         *   not so efficient in memory.
                         */

                        private InventoryManagementStrategyHolder inventoryHolder;

                        protected override int GetSortingOrder()
                        {
                            return 10;
                        }

                        private void Awake()
                        {
                            base.Awake();
                            inventoryHolder = GetComponent<InventoryManagementStrategyHolder>();
                        }

                        private void Start()
                        {
                            inventoryHolder.AddListener(GetComponent<DropLayerInventoryRenderer>());
                        }

                        /************************************************************
                         * Some convenience methods here.
                         ************************************************************/

                        public bool Push(Vector2Int containerPosition, Stack stack, out object finalStackPosition)
                        {
                            return inventoryHolder.Put(containerPosition, null, stack, out finalStackPosition, true);
                        }

                        public Stack Pop(Vector2Int containerPosition)
                        {
                            Stack stack = inventoryHolder.Last(containerPosition);
                            if (stack != null)
                            {
                                inventoryHolder.Remove(containerPosition, stack.QualifiedPosition.First);
                            }
                            return stack;
                        }

                        /************************************************************
                         * Proxy calls to Inventory Holder methods (except for AddListener and related).
                         ************************************************************/

                        public IEnumerable<Tuple<int, Stack>> StackPairs(Vector2Int containerPosition, bool reverse = false)
                        {
                            return from tuple in inventoryHolder.StackPairs(containerPosition, reverse) select new Tuple<int, Stack>((int)tuple.First, tuple.Second);
                        }

                        public Stack Find(Vector2Int containerPosition, int stackPosition)
                        {
                            return inventoryHolder.Find(containerPosition, stackPosition);
                        }

                        public IEnumerable<Stack> FindAll(Vector2Int containerPosition, Func<Tuple<int, Stack>, bool> predicate, bool reverse = false)
                        {
                            return inventoryHolder.FindAll(containerPosition, delegate (Tuple<object, Stack> tuple) { return predicate(new Tuple<int, Stack>((int)tuple.First, tuple.Second)); }, reverse);
                        }

                        public IEnumerable<Stack> FindAll(Vector2Int containerPosition, ScriptableObjects.Inventory.Items.Item item, bool reverse = false)
                        {
                            return inventoryHolder.FindAll(containerPosition, item, reverse);
                        }

                        public Stack First(Vector2Int containerPosition)
                        {
                            return inventoryHolder.First(containerPosition);
                        }

                        public Stack Last(Vector2Int containerPosition)
                        {
                            return inventoryHolder.Last(containerPosition);
                        }

                        public Stack FindOne(Vector2Int containerPosition, Func<Tuple<int, Stack>, bool> predicate, bool reverse = false)
                        {
                            return inventoryHolder.FindOne(containerPosition, delegate (Tuple<object, Stack> tuple) { return predicate(new Tuple<int, Stack>((int)tuple.First, tuple.Second)); }, reverse);
                        }

                        public Stack FindOne(Vector2Int containerPosition, ScriptableObjects.Inventory.Items.Item item, bool reverse = false)
                        {
                            return inventoryHolder.FindOne(containerPosition, item, reverse);
                        }

                        public bool Put(Vector2Int containerPosition, int stackPosition, Stack stack, int? finalStackPosition, bool? optimalPutOnNullPosition = null)
                        {
                            object finalOStackPosition;
                            bool result = inventoryHolder.Put(containerPosition, stackPosition, stack, out finalOStackPosition, optimalPutOnNullPosition);
                            finalStackPosition = finalOStackPosition == null ? null : (int?)finalOStackPosition;
                            return result;
                        }

                        public bool Remove(Vector2Int containerPosition, int stackPosition)
                        {
                            return inventoryHolder.Remove(containerPosition, stackPosition);
                        }

                        public bool Merge(Vector2Int containerPosition, int? destinationStackPosition, int sourceStackPosition)
                        {
                            return inventoryHolder.Merge(containerPosition, destinationStackPosition, sourceStackPosition);
                        }

                        // The other version of `Merge` has little use here.

                        public Stack Take(Vector2Int containerPosition, int stackPosition, object quantity)
                        {
                            return inventoryHolder.Take(containerPosition, stackPosition, quantity);
                        }

                        public bool Split(Vector2Int sourceContainerPosition, int sourceStackPosition, object quantity,
                                          Vector2Int newStackContainerPosition, int newStackPosition, int? finalNewStackPosition)
                        {
                            object finalNewOStackPosition;
                            bool result = inventoryHolder.Split(sourceContainerPosition, sourceStackPosition, quantity,
                                                                newStackContainerPosition, newStackPosition, out finalNewOStackPosition);
                            finalNewStackPosition = finalNewOStackPosition == null ? null : (int?)finalNewOStackPosition;
                            return result;
                        }

                        public bool Use(Vector2Int containerPosition, int sourceStackPosition)
                        {
                            return inventoryHolder.Use(containerPosition, sourceStackPosition);
                        }

                        public bool Use(Vector2Int containerPosition, int sourceStackPosition, object argument)
                        {
                            return inventoryHolder.Use(containerPosition, sourceStackPosition, argument);
                        }

                        public void Clear()
                        {
                            inventoryHolder.Clear();
                        }

                        public void Blink()
                        {
                            inventoryHolder.Blink();
                        }

                        public void Blink(Vector2Int containerPosition)
                        {
                            inventoryHolder.Blink(containerPosition);
                        }

                        public void Blink(Vector2Int containerPosition, int stackPosition)
                        {
                            inventoryHolder.Blink(containerPosition, stackPosition);
                        }

                        public void Import(Types.Inventory.SerializedInventory serializedInventory)
                        {
                            inventoryHolder.Import(serializedInventory);
                        }

                        public Types.Inventory.SerializedInventory Export()
                        {
                            return inventoryHolder.Export();
                        }

                        // Add/Remove listener have little meaning here.
                    }
                }
            }
        }
    }
}
