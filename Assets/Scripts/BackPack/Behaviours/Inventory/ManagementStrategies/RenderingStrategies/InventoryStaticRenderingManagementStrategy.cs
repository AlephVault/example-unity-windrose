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
                    ///   This is a rendering strategy that manages integer positions (from simple spatial
                    ///     strategies) and take from the stacks just the item. No particular rendering
                    ///     strategy is expected here: to just notify the change, any rendering strategy
                    ///     will actually do. Subclasses of this one will perform the actual render, and
                    ///     so they will require a particular definition of what to extract from the
                    ///     per-item rendering strategy. This is a "static" rendering strategy since its
                    ///     representation does not vary with the usage status of the stack.
                    /// </summary>
                    [RequireComponent(typeof(SpatialStrategies.Inventory1DIndexedSpatialManagementStrategy))]
                    public abstract class InventoryStaticRenderingManagementStrategy : InventoryRenderingManagementStrategy
                    {
                        /// <summary>
                        ///   Extracts from the stack its icon, caption and quantity, and converts the stack position to an integer.
                        ///     With that data, invokes <see cref="StackWasUpdated(object, int, Sprite, string, object)"/>.
                        /// </summary>
                        public override void StackWasUpdated(object containerPosition, object stackPosition, Stack stack)
                        {
                            StackWasUpdated(containerPosition, (int)stackPosition, stack.Item, stack.Quantity);
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
                        protected abstract void StackWasUpdated(object containerPosition, object stackPosition, Item item, object quantity);
                    }
                }
            }
        }
    }
}
