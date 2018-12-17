using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                    class ItemUnstackedQuantifyingStrategy : ItemQuantifyingStrategy
                    {
                        /**
                         * Does not make any stack. Just one element per stack.
                         * Quantity is not represented, and cannot added, because it
                         *   is simply 1.
                         */

                        public override StackQuantifyingStrategy CreateStackStrategy(object argument)
                        {
                            return new StackUnstackedQuantifyingStrategy(this, argument);
                        }
                    }
                }
            }
        }
    }
}
