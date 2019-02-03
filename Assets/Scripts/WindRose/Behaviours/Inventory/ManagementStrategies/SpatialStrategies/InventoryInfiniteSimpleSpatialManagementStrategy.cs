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

                    /// <summary>
                    ///   Infinite containers do not have an upper bound. Their sparse array MAY be huge if high indices
                    ///     are occupied.
                    /// </summary>
                    public class InventoryInfiniteSimpleSpatialManagementStrategy : InventorySimpleSpatialManagementStrategy
                    {
                        /// <summary>
                        ///   Infinite simple containers are unbounded.
                        /// </summary>
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

                        /// <summary>
                        ///   Initializes an unbounded container.
                        /// </summary>
                        protected override SpatialContainer InitializeContainer(object position)
                        {
                            return new SimpleInfiniteSpatialContainer(this, position);
                        }

                        /// <summary>
                        ///   The size is always 0 (will count as infinite / unbounded).
                        /// </summary>
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
