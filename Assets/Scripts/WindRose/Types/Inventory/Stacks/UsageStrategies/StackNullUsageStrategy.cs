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

                        public StackNullUsageStrategy(ItemUsageStrategy itemStrategy) : base(itemStrategy)
                        {
                        }

                        public override bool Equals(StackUsageStrategy otherStrategy)
                        {
                            // Yes: an exact class check!
                            return otherStrategy.GetType() == typeof(StackNullUsageStrategy);
                        }
                    }
                }
            }
        }
    }
}
