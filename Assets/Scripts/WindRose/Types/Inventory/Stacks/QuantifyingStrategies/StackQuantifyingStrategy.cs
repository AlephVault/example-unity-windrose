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
                namespace QuantifyingStrategies
                {
                    using ScriptableObjects.Inventory.Items.QuantifyingStrategies;

                    public abstract class StackQuantifyingStrategy : StackStrategy<ItemQuantifyingStrategy>
                    {
                        /**
                         * This stack strategy is related to an ItemQuantifyingStrategy.
                         */
                        public StackQuantifyingStrategy(ItemQuantifyingStrategy itemStrategy, object argument) : base(itemStrategy, argument)
                        {
                        }
                    }
                }
            }
        }
    }
}