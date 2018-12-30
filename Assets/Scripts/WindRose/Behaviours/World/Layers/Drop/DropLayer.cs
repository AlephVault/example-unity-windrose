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
                         * This class is also a listener of all the visual changes 
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
                    }
                }
            }
        }
    }
}
