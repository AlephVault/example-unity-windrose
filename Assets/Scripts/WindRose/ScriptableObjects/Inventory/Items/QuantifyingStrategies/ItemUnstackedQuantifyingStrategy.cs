using UnityEngine;
using WindRose.Types.Inventory.Stacks.QuantifyingStrategies;

namespace WindRose
{
    namespace ScriptableObjects
    {
        namespace Inventory
        {
            namespace Items
            {
                namespace QuantifyingStrategies
                {
                    [CreateAssetMenu(fileName = "NewInventoryItemUnstackedQuantifyingStrategy", menuName = "Wind Rose/Inventory/Item Strategies/Quantifying/Unstacked", order = 101)]
                    public class ItemUnstackedQuantifyingStrategy : ItemQuantifyingStrategy
                    {
                        /**
                         * Does not make any stack. Just one element per stack.
                         * Quantity is not represented, and cannot added, because it
                         *   is simply 1.
                         */

                        public override StackQuantifyingStrategy CreateStackStrategy(object quantity)
                        {
                            return new StackUnstackedQuantifyingStrategy(this);
                        }
                    }
                }
            }
        }
    }
}
