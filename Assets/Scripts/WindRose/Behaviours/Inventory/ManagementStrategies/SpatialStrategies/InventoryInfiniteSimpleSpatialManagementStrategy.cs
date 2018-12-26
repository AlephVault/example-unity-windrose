using System;
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
                namespace SpatialStrategies
                {
                    using ScriptableObjects.Inventory.Items.SpatialStrategies;

                    public class InventoryInfiniteSimpleSpatialManagementStrategy : InventorySimpleSpatialManagementStrategy
                    {
                        /**
                         * This simple container has a non-constrained size.
                         */

                        public class SimpleInfiniteSpatialContainer : SimpleSpatialContainer
                        {
                            public SimpleInfiniteSpatialContainer(InventorySpatialManagementStrategy spatialStrategy, object position) : base(spatialStrategy, position)
                            {
                            }

                            protected override bool ValidateStackPositionAgainstUpperBound(int index)
                            {
                                return true;
                            }
                        }

                        protected override Type GetItemSpatialStrategyCounterpartType()
                        {
                            return typeof(ItemSimpleSpatialStrategy);
                        }

                        protected override SpatialContainer InitializeContainer(object position)
                        {
                            return new SimpleInfiniteSpatialContainer(this, position);
                        }

                        public override int GetSize()
                        {
                            return 0;
                        }
                    }
                }
            }
        }
    }
}
