using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindRose.Types.Inventory.Stacks;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Inventory
        {
            namespace ManagementStrategies
            {
                namespace UsageStrategies
                {
                    using Types.Inventory.Stacks.UsageStrategies;

                    public class InventoryNullUsageManagementStrategy : InventoryUsageManagementStrategy
                    {
                        /**
                         * This strategy does nothing when using a stack (which also has a null strategy there).
                         */

                        protected override IEnumerator DoUse(Types.Inventory.Stacks.Stack stack, object argument)
                        {
                            yield break;
                        }

                        protected override Type GetStackUsageStrategyCounterpartType()
                        {
                            return typeof(StackNullUsageStrategy);
                        }
                    }
                }
            }
        }
    }
}
