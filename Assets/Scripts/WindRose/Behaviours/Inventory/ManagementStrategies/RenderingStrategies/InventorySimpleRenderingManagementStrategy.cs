using System.Collections.Generic;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Inventory
        {
            namespace ManagementStrategies
            {
                namespace RenderingStrategies
                {
                    using Types.Inventory.Stacks;

                    /// <summary>
                    ///   This is a rendering strategy that manages integer positions (from simple spatial
                    ///     strategies) and take from the stacks the icon, caption, and quantity.
                    /// </summary>
                    [RequireComponent(typeof(SpatialStrategies.InventorySimpleSpatialManagementStrategy))]
                    public abstract class InventorySimpleRenderingManagementStrategy : InventoryRenderingManagementStrategy
                    {
                        /// <summary>
                        ///   The related (required) simple spatial strategy.
                        /// </summary>
                        protected SpatialStrategies.InventorySimpleSpatialManagementStrategy spatialStrategy;

                        protected override void Awake()
                        {
                            base.Awake();
                            spatialStrategy = GetComponent<SpatialStrategies.InventorySimpleSpatialManagementStrategy>();
                        }

                        /// <summary>
                        ///   Extracts from the stack its icon, caption and quantity, and converts the stack position to an integer.
                        ///     With that data, invokes <see cref="StackWasUpdated(object, int, Sprite, string, object)"/>.
                        /// </summary>
                        public override void StackWasUpdated(object containerPosition, object stackPosition, Stack stack)
                        {
                            Dictionary<string, object> target = new Dictionary<string, object>();
                            stack.MainRenderingStrategy.DumpRenderingData(target);
                            StackWasUpdated(containerPosition, (int)stackPosition, (Sprite)target["icon"], (string)target["caption"], target["quantity"]);
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
                        /// <param name="icon">The stack's icon</param>
                        /// <param name="caption">The stack's caption</param>
                        /// <param name="quantity">The stack's quantity</param>
                        protected abstract void StackWasUpdated(object containerPosition, int stackPosition, Sprite icon, string caption, object quantity);

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
