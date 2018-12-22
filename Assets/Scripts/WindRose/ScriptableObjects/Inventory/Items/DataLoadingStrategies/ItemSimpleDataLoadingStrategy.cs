using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace WindRose
{
    namespace ScriptableObjects
    {
        namespace Inventory
        {
            namespace Items
            {
                namespace DataLoadingStrategies
                {
                    using Types.Inventory.Stacks.DataDumpingStrategies;
                    using QuantifyingStrategies;
                    using UsageStrategies;

                    [CreateAssetMenu(fileName = "NewInventoryItemSimpleDataLoadingStrategy", menuName = "Wind Rose/Inventory/Item Strategies/Data Loading/Simple", order = 101)]
                    public class ItemSimpleDataLoadingStrategy : ItemDataLoadingStrategy
                    {
                        /**
                         * This strategy only accounts for the quantity. Related items will
                         *   will br critical or consumable items, but the related usage
                         *   strategies will have to expect no data. The source object
                         *   should be a dictionary with a key ["quantity"].
                         */
                        
                        public override StackDataDumpingStrategy CreateStackStrategy(object argument)
                        {
                            return new StackSimpleDataDumpingStrategy(this, argument);
                        }

                        public override object LoadDataFor(ItemQuantifyingStrategy strategy, object source)
                        {
                            // We get ["quantity"] as its quantity
                            if (source is Dictionary<string, object>) {
                                object quantity;
                                ((Dictionary<string, object>)source).TryGetValue("quantity", out quantity);
                                return quantity;
                            }
                            return null;
                        }

                        public override object LoadDataFor(ItemUsageStrategy strategy, object source)
                        {
                            // We pass no data to usage strategies. Related usage strategies
                            //   MUST be able to expect `null` as a possible import value.
                            return null;
                        }
                    }
                }
            }
        }
    }
}
