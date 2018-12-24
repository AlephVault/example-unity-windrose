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

                        public interface ISimpleInventoryRenderer
                        {
                            void Connected(InventoryManagementStrategyHolder holder, int maxSize);
                            void RefreshStack(object containerPosition, object stackPosition, Sprite icon, string caption, object quantity);
                            void RemoveStack(object containerPosition, object stackPosition);
                            void Clear();
                            void Disconnected();
                        }

                        protected override bool AllowsListener(MonoBehaviour listener)
                        {
                            return listener is ISimpleInventoryRenderer;
                        }

                        protected override void ListenerHasBeenAdded(MonoBehaviour listener)
                        {
                            ((ISimpleInventoryRenderer)listener).Connected(StrategyHolder, spatialStrategy.Size);
                        }

                        protected override void ListenerHasBeenRemoved(MonoBehaviour listener)
                        {
                            ((ISimpleInventoryRenderer)listener).Disconnected();
                        }

                        protected override void StackWasUpdated(MonoBehaviour listener, object containerPosition, object stackPosition, Stack stack)
                        {
                            Dictionary<string, object> target = new Dictionary<string, object>();
                            stack.MainRenderingStrategy.DumpRenderingData(target);
                            ((ISimpleInventoryRenderer)listener).RefreshStack(containerPosition, stackPosition, (Sprite)target["icon"], (string)target["caption"], target["quantity"]);
                        }

                        protected override void StackWasRemoved(MonoBehaviour listener, object containerPosition, object stackPosition)
                        {
                            ((ISimpleInventoryRenderer)listener).RemoveStack(containerPosition, stackPosition);
                        }

                        protected override void EverythingWasCleared(MonoBehaviour listener)
                        {
                            ((ISimpleInventoryRenderer)listener).Clear();
                        }
                    }
                }
            }
        }
    }
}
