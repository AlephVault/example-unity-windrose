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
                        public StackUsageStrategy(ItemUsageStrategy itemStrategy, object argument) : base(itemStrategy, argument)
                        {
                        }

                        /**
                         * Clones a usage strategy. Useful for cloning or splitting stacks.
                         */
                        public StackUsageStrategy Clone()
                        {
                            return ItemStrategy.CreateStackStrategy(Export());
                        }

                        /**
                         * For exatly this reason, the usage strategies may depend on the Quantifying strategies.
                         * By interpolating I mean: certain strategies may calculate new intermediate values for the interpolated given
                         *   the quantities. Others may instead require that both strategies are of the same type AND VALUES.
                         * 
                         * You can deny interpolation by returning null instead of a new instance of stack usage strategy.
                         * 
                         * Example: you can interpolate 1kg green powder + 1kg yellow powder by returning 2kg green powder.
                         * 
                         * The return value is an action (an empty procedure) that, when executed, will apply all the changes. You
                         *   must not execute the changes directly, but return a delegate(){} that performs them.
                         */
                        public abstract Action Interpolate(StackUsageStrategy otherStrategy, object currentQuantity, object addedQuantity);
                    }
                }
            }
        }
    }
}
