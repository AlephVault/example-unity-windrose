using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindRose
{
    namespace Types
    {
        namespace Inventory
        {
            namespace Stacks
            {
                namespace SpatialStrategies
                {
                    using Behaviours.Inventory.ManagementStrategies.SpatialStrategies;
                    using ScriptableObjects.Inventory.Items.SpatialStrategies;

                    public class StackSpatialStrategy : StackStrategy<ItemSpatialStrategy>
                    {
                        /**
                         * This stack strategy is related to an ItemSpatialStrategy. It will be concrete,
                         *   since its position does not depend on anything but the inventory it is
                         *   added to.
                         */

                        public InventorySpatialManagementStrategy.QualifiedStackPosition QualifiedPosition
                        {
                            get; private set;
                        }

                        public StackSpatialStrategy(ItemSpatialStrategy itemStrategy) : base(itemStrategy)
                        {
                        }

                        /**
                         * Clones a spatial strategy. Useful for cloning or splitting stacks.
                         * Actualy, no cloning here. Just creating a spatial strategy with no position.
                         */
                        public StackSpatialStrategy Clone()
                        {
                            return ItemStrategy.CreateStackStrategy();
                        }
                    }
                }
            }
        }
    }
}