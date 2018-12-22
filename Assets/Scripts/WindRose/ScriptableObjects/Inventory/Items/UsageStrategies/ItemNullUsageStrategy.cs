using System;
using UnityEngine;


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

                    [CreateAssetMenu(fileName = "NewInventoryItemNullUsageStrategy", menuName = "Wind Rose/Inventory/Item Strategies/Usage/Null Usage (e.g. tokens, critical objects)", order = 101)]
                    public class ItemNullUsageStrategy : ItemUsageStrategy
                    {
                        /**
                         * This strategy is for items that cannot be used. This means:
                         *   items that, when you attempt to use them, do nothing. One
                         *   useful example is critical objects.
                         */

                        public override StackUsageStrategy CreateStackStrategy(object argument)
                        {
                            return new StackNullUsageStrategy(this, argument);
                        }
                    }
                }
            }
        }
    }
}
