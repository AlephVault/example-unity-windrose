using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackPack
{
	using Types.Inventory.Stacks;

    namespace Behaviours
    {
        namespace Inventory
        {
            namespace ManagementStrategies
            {
                namespace UsageStrategies
                {
                    using System.Threading.Tasks;
                    using Types.Inventory.Stacks.UsageStrategies;

                    /// <summary>
                    ///   This usage strategy is dummy. Accepts any usage strategy, but does nothing
                    ///     when trying to use any stack.
                    /// </summary>
                    public class InventoryNullUsageManagementStrategy : InventoryUsageManagementStrategy
                    {
                        protected override async Task DoUse(Types.Inventory.Stacks.Stack stack, object argument) {}

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
