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
                    using ScriptableObjects.Inventory.Items.SpatialStrategies;

                    public abstract class StackSpatialStrategy : StackStrategy<ItemSpatialStrategy>
                    {
                        /**
                         * This stack strategy is related to an ItemSpatialStrategy.
                         */
                        public StackSpatialStrategy(ItemSpatialStrategy itemStrategy, object argument) : base(itemStrategy, argument)
                        {
                        }
                    }
                }
            }
        }
    }
}