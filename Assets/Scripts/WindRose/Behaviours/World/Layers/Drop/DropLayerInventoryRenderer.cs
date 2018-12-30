using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using WindRose.Behaviours.Drops;
using WindRose.Behaviours.Inventory;
using WindRose.Behaviours.Inventory.ManagementStrategies.RenderingStrategies;

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
                    [RequireComponent(typeof(DropLayer))]
                    public class DropLayerInventoryRenderer : InventorySimpleRenderingManagementStrategy.SimpleInventoryRenderer
                    {
                        private SimpleDropContainerRenderer[,] dropContainers;
                        private InventoryManagementStrategyHolder inventoryHolder;
                        private InventoryMapSizedPositioningManagementStrategy positioningStrategy;

                        /**
                         * This is a prefab you have to set. See more details in the `SimpleDropContainer` class.
                         */
                        [SerializeField]
                        private SimpleDropContainerRenderer containerPrefab;

                        private void Awake()
                        {
                            positioningStrategy = GetComponent<InventoryMapSizedPositioningManagementStrategy>();
                            inventoryHolder = GetComponent<InventoryManagementStrategyHolder>();
                        }

                        private void Start()
                        {
                            Map map = GetComponent<DropLayer>().Map;
                            dropContainers = new SimpleDropContainerRenderer[map.Width, map.Height];
                        }

                        public override void Connected(InventoryManagementStrategyHolder holder, int maxSize)
                        {
                            // Nothing happens here in particular.
                        }

                        public override void Disconnected()
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
                                container.transform.localPosition = new Vector3(position.x, position.y);
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

                        public override void RefreshStack(object containerPosition, object stackPosition, Sprite icon, string caption, object quantity)
                        {
                            // Adds a stack to a container (creates the container if absent).
                            Vector2Int containerVector = (Vector2Int)containerPosition;
                            int stackIndex = (int)stackPosition;
                            SimpleDropContainerRenderer container = getContainerFor(containerVector, true);
                            container.RefreshWithPutting(stackIndex, icon, caption, quantity);
                        }

                        public override void RemoveStack(object containerPosition, object stackPosition)
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

                        public override void Clear()
                        {
                            // Destroys all the containers.
                            foreach (object position in positioningStrategy.Positions())
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
