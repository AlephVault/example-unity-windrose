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
                namespace DataDumpingStrategies
                {
                    using ScriptableObjects.Inventory.Items.DataLoadingStrategies;
                    using QuantifyingStrategies;
                    using UsageStrategies;

                    public class StackSimpleDataDumpingStrategy : StackDataDumpingStrategy
                    {
                        /**
                         * This strategy, as the counterpart for the item strategy, just dumps the quantity into a
                         *   dictionary. As in its counterpart, we don't consider usage strategies have relevant
                         *   data here.
                         */
                        
                        public StackSimpleDataDumpingStrategy(ItemDataLoadingStrategy itemStrategy, object argument) : base(itemStrategy, argument)
                        {
                        }

                        public override void DumpDataFor(StackUsageStrategy strategy, object exported, object target)
                        {
                        }

                        public override void DumpDataFor(StackQuantifyingStrategy strategy, object exported, object target)
                        {
                            if (!(strategy is StackUnstackedQuantifyingStrategy))
                            {
                                ((Dictionary<string, object>)target)["quantity"] = strategy.Quantity;
                            }
                        }
                    }
                }
            }
        }
    }
}
