using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindRose
{
    namespace ScriptableObjects
    {
        namespace Inventory
        {
            namespace Items
            {
                namespace SpatialStrategies
                {
                    using Types.Inventory.Stacks.SpatialStrategies;

                    public class ItemSimpleSpatialStrategy : StackSpatialStrategy
                    {
                        /**
                         * This is a simple spatial strategy that indexes
                         *   its content. Most games using inventory make
                         *   use of this approach: each item stack will
                         *   be contained in one single box.
                         * 
                         * This strategy needs no additional data (other
                         *   strategies may involve weight or dimensions).
                         */

                        public ItemSimpleSpatialStrategy(ItemSpatialStrategy itemStrategy, object argument) : base(itemStrategy, argument)
                        {
                        }
                    }
                }
            }
        }
    }
}
