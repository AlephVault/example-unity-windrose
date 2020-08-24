using System.Collections.Generic;
using UnityEngine;

namespace BackPack
{
    namespace Behaviours
    {
        namespace Inventory
        {
            namespace ManagementStrategies
            {
                namespace RenderingStrategies
                {
                    using ScriptableObjects.Inventory.Items;
                    using Types.Inventory.Stacks;

                    /// <summary>
                    ///   This is a rendering strategy that manages integer positions and performs
                    ///     the static rendering: just passing item and quantity. Subclasses will
                    ///     know how to render the item, and will typically involve an Icon/Text
                    ///     approach, with no possibility of changing representation state.
                    /// </summary>
                    [RequireComponent(typeof(SpatialStrategies.Inventory1DIndexedSpatialManagementStrategy))]
                    public abstract class Inventory1DIndexedStaticRenderingManagementStrategy : InventoryStaticRenderingManagementStrategy
                    {
                        /// <summary>
                        ///   The related (required) 1D indexed spatial strategy.
                        /// </summary>
                        protected SpatialStrategies.Inventory1DIndexedSpatialManagementStrategy spatialStrategy;

                        protected override void Awake()
                        {
                            base.Awake();
                            spatialStrategy = GetComponent<SpatialStrategies.Inventory1DIndexedSpatialManagementStrategy>();
                        }

                        /// <summary>
                        ///   Extracts from the stack its icon, caption and quantity, and converts the stack position to an integer.
                        ///     With that data, invokes <see cref="StackWasUpdated(object, int, Sprite, string, object)"/>.
                        /// </summary>
                        protected override void StackWasUpdated(object containerPosition, object stackPosition, Item item, object quantity)
                        {
                            StackWasUpdated(containerPosition, (int)stackPosition, item, quantity);
                        }

                        /// <summary>
                        ///   Converts the stack position to an integer. With that data, invokes
                        ///     <see cref="StackWasRemoved(object, int)"/>.
                        /// </summary>
                        public override void StackWasRemoved(object containerPosition, object stackPosition)
                        {
                            StackWasRemoved(containerPosition, (int)stackPosition);
                        }

                        /// <summary>
                        ///   Modified version of <see cref="StackWasUpdated(object, object, Stack)"/> that processes
                        ///     the particular data: icon, caption, quantity in integer positions.
                        /// </summary>
                        /// <param name="containerPosition">The ID of the container the stack is added into</param>
                        /// <param name="stackPosition">The position in the container the stack is added into</param>
                        /// <param name="item">The stack's item</param>
                        /// <param name="quantity">The stack's quantity</param>
                        protected abstract void StackWasUpdated(object containerPosition, int stackPosition, Item item, object quantity);

                        /// <summary>
                        ///   Modified version of <see cref="StackWasRemoved(object, object)"/> that processes
                        ///     integer positions
                        /// </summary>
                        /// <param name="containerPosition">The ID of the container the stack is removed from</param>
                        /// <param name="stackPosition">The position in the container the stack is removed from</param>
                        protected abstract void StackWasRemoved(object containerPosition, int stackPosition);
                    }
                }
            }
        }
    }
}
