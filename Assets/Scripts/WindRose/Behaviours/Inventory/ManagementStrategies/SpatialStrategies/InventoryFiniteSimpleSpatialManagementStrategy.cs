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

                    public class InventoryFiniteSimpleSpatialManagementStrategy : InventorySimpleSpatialManagementStrategy
                    {
                        /**
                         * This simple container has a constrained size.
                         */

                        public class SimpleFiniteSpatialContainer : SimpleSpatialContainer
                        {
                            public SimpleFiniteSpatialContainer(InventorySpatialManagementStrategy spatialStrategy, object position) : base(spatialStrategy, position)
                            {
                            }

                            protected override bool ValidateStackPositionAgainstUpperBound(int index)
                            {
                                return index < ((InventoryFiniteSimpleSpatialManagementStrategy)SpatialStrategy).Size;
                            }
                        }

                        [SerializeField]
                        private int size = 0;

                        public int Size
                        {
                            get { return size; }
                        }

                        protected void Awake()
                        {
                            base.Awake();
                            if (size < 0)
                            {
                                size = 0;
                            }
                        }

                        protected override Type GetItemSpatialStrategyCounterpartType()
                        {
                            return typeof(ItemSimpleSpatialStrategy);
                        }

                        protected override SpatialContainer InitializeContainer(object position)
                        {
                            return new SimpleFiniteSpatialContainer(this, position);
                        }
                    }
                }
            }
        }
    }
}
