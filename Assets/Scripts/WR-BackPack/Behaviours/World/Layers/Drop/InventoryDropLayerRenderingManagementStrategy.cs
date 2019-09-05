using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
		using Drops;
		using BackPack.Behaviours.Inventory;
		using BackPack.Behaviours.Inventory.ManagementStrategies.RenderingStrategies;

        namespace World
        {
            namespace Layers
            {
                namespace Drop
                {
                    /// <summary>
                    ///   This strategy renders a matrix of M x N containers, since it will be related to a map's
                    ///     <see cref="DropLayer"/>. It will do this by creating/refreshing/destroying a lot of
                    ///     <see cref="SimpleDropContainerRenderer"/> instances (one on each map's position).
                    /// </summary>
                    public class InventoryDropLayerRenderingManagementStrategy : InventorySimpleRenderingManagementStrategy
                    {
                        private SimpleDropContainerRenderer[,] dropContainers;
                        // We are completely sure we have a PositioningStrategy in the underlying object
                        private InventoryMapSizedPositioningManagementStrategy positioningStrategy;

                        /// <summary>
                        ///   A prefab that MUST be set. It will be used to spawn the renderers for the
                        ///     drop containers.
                        /// </summary>
                        [SerializeField]
                        private SimpleDropContainerRenderer containerPrefab;

                        protected override void Awake()
                        {
                            base.Awake();
                            positioningStrategy = GetComponent<InventoryMapSizedPositioningManagementStrategy>();
                        }

                        private void Start()
                        {
                            try
                            {
                                Map map = GetComponent<DropLayer>().Map;
                                dropContainers = new SimpleDropContainerRenderer[map.Width, map.Height];
                            }
                            catch (NullReferenceException)
                            {
                                Debug.Log("This rendering strategy must be bound to an object being also a DropLayer");
                                Destroy(gameObject);
                            }
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

                        /// <summary>
                        ///   <para>
                        ///     Refreshing the stack involves maybe-instantiating a container renderer, and then
                        ///       refresh-putting an item there.
                        ///   </para>
                        ///   <para>
                        ///     See <see cref="InventoryRenderingManagementStrategy.StackWasUpdated(object, object, Types.Inventory.Stacks.Stack)"/>
                        ///       for more information on the method's signature.
                        ///   </para>
                        /// </summary>
                        /// <param name="containerPosition"></param>
                        /// <param name="stackPosition"></param>
                        /// <param name="icon"></param>
                        /// <param name="caption"></param>
                        /// <param name="quantity"></param>
                        protected override void StackWasUpdated(object containerPosition, int stackPosition, Sprite icon, string caption, object quantity)
                        {
                            // Adds a stack to a container (creates the container if absent).
                            Vector2Int containerVector = (Vector2Int)containerPosition;
                            SimpleDropContainerRenderer container = getContainerFor(containerVector, true);
                            container.RefreshWithPutting(stackPosition, icon, caption, quantity);
                        }

                        /// <summary>
                        ///   <para>
                        ///     Removing a stack involves removing the item from the container, and then maybe-destroying
                        ///       the container if empty.
                        ///   </para>
                        ///   <para>
                        ///     See <see cref="InventoryRenderingManagementStrategy.StackWasRemoved(object, object)"/>
                        ///       for more information on the method's signature.
                        ///   </para>
                        /// </summary>
                        /// <param name="containerPosition"></param>
                        /// <param name="stackPosition"></param>
                        protected override void StackWasRemoved(object containerPosition, int stackPosition)
                        {
                            // Removes a stack from a container (if the container exists).
                            // Removes the container if empty.
                            Vector2Int containerVector = (Vector2Int)containerPosition;
                            SimpleDropContainerRenderer container = getContainerFor(containerVector, false);
                            if (container != null)
                            {
                                container.RefreshWithRemoving(stackPosition);
                                if (container.Empty())
                                {
                                    Destroy(container);
                                    dropContainers[containerVector.x, containerVector.y] = null;
                                }
                            }
                        }

                        /// <summary>
                        ///   Clearing all the containers involves destroying the renderer objects.
                        /// </summary>
                        public override void EverythingWasCleared()
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
