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

                        public StackSpatialStrategy(ItemSpatialStrategy itemStrategy, object argument) : base(itemStrategy, argument)
                        {
                        }

                        protected override void Import(object argument)
                        {
                            /*
                             * NOTES: By default, no argument will be specified to the spatial strategy.
                             *   This is because the spatial strategy is inventory-specific, and not stack-specific.
                             *
                             * Inventories are responsible of setting the appropriate position and checking its type.
                             * When a stack is created by an item, It will have this strategy as well (to be able to
                             *   retrieve its dimensions, if appropriate), but will have no position.
                             */
                            QualifiedPosition = null;
                        }

                        public override object Export()
                        {
                            return null;
                        }

                        /**
                         * Clones a spatial strategy. Useful for cloning or splitting stacks.
                         * Actualy, no cloning here. Just creating a spatial strategy with no position.
                         */
                        public StackSpatialStrategy Clone()
                        {
                            return ItemStrategy.CreateStackStrategy(null);
                        }
                    }
                }
            }
        }
    }
}