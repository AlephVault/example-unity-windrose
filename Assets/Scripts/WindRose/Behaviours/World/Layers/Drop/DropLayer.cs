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
                    using Drops;

                    [RequireComponent(typeof(InventoryMapSizedPositioningManagementStrategy))]
                    [RequireComponent(typeof(InventorySimpleRenderingManagementStrategy))]
                    [RequireComponent(typeof(InventoryInfiniteSimpleSpatialManagementStrategy))]
                    [RequireComponent(typeof(InventoryManagementStrategyHolder))]
                    public class DropLayer : MapLayer, InventorySimpleRenderingManagementStrategy.ISimpleInventoryRenderer
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

                        private SimpleDropContainerRenderer[,] dropContainers;
                        private InventoryManagementStrategyHolder inventoryHolder;
                        private InventoryMapSizedPositioningManagementStrategy positioningStrategy;

                        protected override int GetSortingOrder()
                        {
                            return 10;
                        }

                        protected override void Awake()
                        {
                            base.Awake();
                            positioningStrategy = GetComponent<InventoryMapSizedPositioningManagementStrategy>();
                            inventoryHolder = GetComponent<InventoryManagementStrategyHolder>();
                            dropContainers = new SimpleDropContainerRenderer[Map.Width, Map.Height];
                        }

                        /**
                         * This is a prefab you have to set. See more details in the `SimpleDropContainer` class.
                         */
                        [SerializeField]
                        private SimpleDropContainerRenderer containerPrefab;

                        /************************************************************
                         * This class is also a listener of all the visual changes 
                         ************************************************************/

                        public bool Push(Vector2Int containerPosition, Stack stack, out object finalStackPosition)
                        {
                            return inventoryHolder.Put(containerPosition, null, stack, out finalStackPosition, true);
                        }

                        public Stack Pop(Vector2Int containerPosition)
                        {
                            Stack stack = inventoryHolder.First(containerPosition);
                            if (stack != null)
                            {
                                inventoryHolder.Remove(containerPosition, stack.QualifiedPosition.First);
                            }
                            return stack;
                        }

                        /************************************************************
                         * This class is also a listener of all the visual changes 
                         ************************************************************/

                        public void Connected(InventoryManagementStrategyHolder holder, int maxSize)
                        {
                            // Nothing happens here in particular.
                        }

                        public void Disconnected()
                        {
                            Clear();
                        }

                        private SimpleDropContainerRenderer getContainerFor(Vector2Int position, bool createIfMissing = true)
                        {
                            // Retrieves, or clones (if createIsMissing), a container for a specific (x, y) position
                            SimpleDropContainerRenderer container = dropContainers[position.x, position.y];
                            if (container == null && createIfMissing)
                            {
                                container = Instantiate(containerPrefab.gameObject, transform).GetComponent<SimpleDropContainerRenderer>();
                                dropContainers[position.x, position.y] = container;
                                container.transform.position = new Vector3(position.x, position.y);
                            }
                            return container;
                        }

                        private void destroyContainerFor(Vector2Int position)
                        {
                            // Destroys a container, if existing, at an (x, y) position
                            SimpleDropContainerRenderer container = dropContainers[position.x, position.y];
                            if (container != null)
                            {
                                Destroy(container);
                                dropContainers[position.x, position.y] = null;
                            }
                        }

                        public void RefreshStack(object containerPosition, object stackPosition, Sprite icon, string caption, object quantity)
                        {
                            // Adds a stack to a container (creates the container if absent).
                            Vector2Int containerVector = (Vector2Int)containerPosition;
                            int stackIndex = (int)stackPosition;
                            SimpleDropContainerRenderer container = getContainerFor(containerVector, false);
                            container.RefreshWithPutting(stackIndex, icon, caption, quantity);
                        }

                        public void RemoveStack(object containerPosition, object stackPosition)
                        {
                            // Removes a stack from a container (if the container exists).
                            // Removes the container if empty.
                            Vector2Int containerVector = (Vector2Int)containerPosition;
                            int stackIndex = (int)stackPosition;
                            SimpleDropContainerRenderer container = getContainerFor(containerVector, false);
                            if (container != null)
                            {
                                container.RefreshWithRemoving(stackIndex);
                                if (container.Empty())
                                {
                                    Destroy(container);
                                    dropContainers[containerVector.x, containerVector.y] = null;
                                }
                            }
                        }

                        public void Clear()
                        {
                            // Destroys all the containers.
                            foreach(object position in positioningStrategy.Positions())
                            {
                                destroyContainerFor((Vector2Int)position);
                            }
                        }
                    }
                }
            }
        }
    }
}
