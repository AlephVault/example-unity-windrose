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
                    public class InventorySimpleRenderingManagementStrategy : InventoryRenderingManagementStrategy
                    {
                        /**
                         * This strategy works with the following data from simple rendering strategies:
                         *   icon, caption, quantity.
                         * 
                         * Accepted behaviours are thos ones implementing the interface we are defining
                         *   here, in this particular class: ISimpleInventoryRenderer.
                         * 
                         * The only required behaviour here, is a simple spatial strategy component.
                         */

                        private SpatialStrategies.InventorySimpleSpatialManagementStrategy spatialStrategy;

                        private void Awake()
                        {
                            spatialStrategy = GetComponent<SpatialStrategies.InventorySimpleSpatialManagementStrategy>();
                        }

                        public abstract class SimpleInventoryRenderer : MonoBehaviour
                        {
                            public abstract void Connected(InventoryManagementStrategyHolder holder, int maxSize);
                            public abstract void RefreshStack(object containerPosition, object stackPosition, Sprite icon, string caption, object quantity);
                            public abstract void RemoveStack(object containerPosition, object stackPosition);
                            public abstract void Clear();
                            public abstract void Disconnected();
                        }

                        protected override bool AllowsListener(MonoBehaviour listener)
                        {
                            return listener is SimpleInventoryRenderer;
                        }

                        protected override void ListenerHasBeenAdded(MonoBehaviour listener)
                        {
                            ((SimpleInventoryRenderer)listener).Connected(StrategyHolder, spatialStrategy.GetSize());
                        }

                        protected override void ListenerHasBeenRemoved(MonoBehaviour listener)
                        {
                            ((SimpleInventoryRenderer)listener).Disconnected();
                        }

                        protected override void StackWasUpdated(MonoBehaviour listener, object containerPosition, object stackPosition, Stack stack)
                        {
                            Dictionary<string, object> target = new Dictionary<string, object>();
                            stack.MainRenderingStrategy.DumpRenderingData(target);
                            ((SimpleInventoryRenderer)listener).RefreshStack(containerPosition, stackPosition, (Sprite)target["icon"], (string)target["caption"], target["quantity"]);
                        }

                        protected override void StackWasRemoved(MonoBehaviour listener, object containerPosition, object stackPosition)
                        {
                            ((SimpleInventoryRenderer)listener).RemoveStack(containerPosition, stackPosition);
                        }

                        protected override void EverythingWasCleared(MonoBehaviour listener)
                        {
                            ((SimpleInventoryRenderer)listener).Clear();
                        }
                    }
                }
            }
        }
    }
}
