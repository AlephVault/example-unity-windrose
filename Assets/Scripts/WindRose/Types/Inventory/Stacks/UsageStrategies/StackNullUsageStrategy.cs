using System;
using WindRose.ScriptableObjects.Inventory.Items.UsageStrategies;

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
                    class StackNullUsageStrategy : StackUsageStrategy
                    {
                        /**
                         * Stacks with this strategy do nothing when attempted to use them. In the same way, interpolation
                         *   will be trivial and always succeed.
                         */

                        public StackNullUsageStrategy(ItemUsageStrategy itemStrategy, object argument) : base(itemStrategy, argument)
                        {
                        }

                        public override Action Interpolate(StackUsageStrategy otherStrategy, object currentQuantity, object addedQuantity)
                        {
                            return delegate () {};
                        }
                    }
                }
            }
        }
    }
}
