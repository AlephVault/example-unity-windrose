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

                    [RequireComponent(typeof(SpatialStrategies.InventorySimpleSpatialManagementStrategy))]
                    public abstract class InventorySimpleRenderingManagementStrategy : InventoryRenderingManagementStrategy
                    {
                        /**
                         * This strategy works with the following data from simple rendering strategies:
                         *   icon, caption, quantity.
                         * 
                         * The only required behaviour here, is a simple spatial strategy component.
                         * 
                         * The arguments are preprocessed and changed into image, description and quantity.
                         * Stack position will also be converted: the new method will return an integer value.
                         */

                        protected SpatialStrategies.InventorySimpleSpatialManagementStrategy spatialStrategy;

                        private void Awake()
                        {
                            spatialStrategy = GetComponent<SpatialStrategies.InventorySimpleSpatialManagementStrategy>();
                        }

                        public override void StackWasUpdated(object containerPosition, object stackPosition, Stack stack)
                        {
                            Dictionary<string, object> target = new Dictionary<string, object>();
                            stack.MainRenderingStrategy.DumpRenderingData(target);
                            StackWasUpdated(containerPosition, (int)stackPosition, (Sprite)target["icon"], (string)target["caption"], target["quantity"]);
                        }

                        public override void StackWasRemoved(object containerPosition, object stackPosition)
                        {
                            StackWasRemoved(containerPosition, (int)stackPosition);
                        }

                        protected abstract void StackWasUpdated(object containerPosition, int stackPosition, Sprite icon, string caption, object quantity);
                        protected abstract void StackWasRemoved(object containerPosition, int stackPosition);
                    }
                }
            }
        }
    }
}
