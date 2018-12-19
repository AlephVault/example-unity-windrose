using System;

namespace WindRose
{
    namespace ScriptableObjects
    {
        namespace Inventory
        {
            namespace Items
            {
                namespace UsageStrategies
                {
                    using Types.Inventory.Stacks.UsageStrategies;

                    public class ItemNullUsageStrategy : ItemUsageStrategy
                    {
                        /**
                         * This strategy is for items that cannot be used. This means:
                         *   items that, when you attempt to use them, do nothing.
                         */

                        public override StackUsageStrategy CreateStackStrategy(object argument)
                        {
                            throw new NotImplementedException();
                        }
                    }
                }
            }
        }
    }
}
