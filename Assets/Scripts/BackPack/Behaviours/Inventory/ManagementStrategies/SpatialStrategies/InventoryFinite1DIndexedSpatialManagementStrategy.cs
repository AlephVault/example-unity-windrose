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
                    ///   Finite 1D indexed spatial management strategies have a limit on the elements that can be
                    ///     added (indexes).
                    /// </summary>
                    public class InventoryFinite1DIndexedSpatialManagementStrategy : Inventory1DIndexedSpatialManagementStrategy
                    {
                        /// <summary>
                        ///   This spatial container checks bounds to disallow arbitrarily large
                        ///     containers.
                        /// </summary>
                        public class SimpleFiniteSpatialContainer : Container
                        {
                            public SimpleFiniteSpatialContainer(InventorySpatialManagementStrategy spatialStrategy, object position) : base(spatialStrategy, position)
                            {
                            }

                            /// <summary>
                            ///   Bound-check against the in-strategy maximum.
                            /// </summary>
                            protected override bool ValidateStackPositionAgainstUpperBound(int index)
                            {
                                return index < ((InventoryFinite1DIndexedSpatialManagementStrategy)SpatialStrategy).Size;
                            }
                        }

                        /// <summary>
                        ///   The maximum size. It will be clamped to 1 if less than 1.
                        /// </summary>
                        [SerializeField]
                        private int size = 0;

                        /// <summary>
                        ///   See <see cref="size"/>.
                        /// </summary>
                        public int Size
                        {
                            get { return size; }
                        }

                        protected override void Awake()
                        {
                            base.Awake();
                            if (size <= 0)
                            {
                                size = 1;
                            }
                        }

                        /// <summary>
                        ///   The container type being initializes is the simple+finite one.
                        /// </summary>
                        protected override SpatialContainer InitializeContainer(object position)
                        {
                            return new SimpleFiniteSpatialContainer(this, position);
                        }

                        /// <summary>
                        ///   Getting the size is done directly from the <see cref="Size"/> property.
                        /// </summary>
                        public override int GetSize()
                        {
                            return Size;
                        }
                    }
                }
            }
        }
    }
}
