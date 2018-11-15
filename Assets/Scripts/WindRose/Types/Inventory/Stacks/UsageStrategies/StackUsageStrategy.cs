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
                namespace UsageStrategies
                {
                    using ScriptableObjects.Inventory.Items.UsageStrategies;

                    public abstract class StackUsageStrategy : StackStrategy<ItemUsageStrategy>
                    {
                        /**
                         * This stack strategy is related to an ItemUsageStrategy.
                         */
                        public StackUsageStrategy(ItemUsageStrategy itemStrategy, Dictionary<string, object> arguments) : base(itemStrategy, arguments)
                        {
                        }
                    }
                }
            }
        }
    }
}
