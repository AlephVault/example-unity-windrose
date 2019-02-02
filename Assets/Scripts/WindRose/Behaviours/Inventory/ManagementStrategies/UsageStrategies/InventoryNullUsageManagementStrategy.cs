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

                    /// <summary>
                    ///   This usage strategy is dummy. Accepts any usage strategy, but does nothing
                    ///     when trying to use any stack.
                    /// </summary>
                    public class InventoryNullUsageManagementStrategy : InventoryUsageManagementStrategy
                    {
                        protected override IEnumerator DoUse(Types.Inventory.Stacks.Stack stack, object argument)
                        {
                            yield break;
                        }

                        public override bool Accepts(StackUsageStrategy strategy)
                        {
                            return true;
                        }
                    }
                }
            }
        }
    }
}
