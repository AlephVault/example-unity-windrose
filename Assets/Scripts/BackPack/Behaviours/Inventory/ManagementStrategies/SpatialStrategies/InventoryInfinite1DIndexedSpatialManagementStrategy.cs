using System;
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
                namespace SpatialStrategies
                {
                    using ScriptableObjects.Inventory.Items.SpatialStrategies;

                    /// <summary>
                    ///   Infinite containers do not have an upper bound. Their sparse array MAY be huge if high indices
                    ///     are occupied.
                    /// </summary>
                    public class InventoryInfinite1DIndexedSpatialManagementStrategy : Inventory1DIndexedSpatialManagementStrategy
                    {
                        /// <summary>
                        ///   Infinite 1D indexed containers are unbounded.
                        /// </summary>
                        public new class Container : Inventory1DIndexedSpatialManagementStrategy.Container
                        {
                            public Container(InventorySpatialManagementStrategy spatialStrategy, object position) : base(spatialStrategy, position)
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
                            return new Container(this, position);
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
