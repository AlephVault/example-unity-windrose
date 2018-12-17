using System;
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
                    class ItemIntegerQuantifyingStrategy : ItemQuantifyingStrategy
                    {
                        /**
                         * Makes stacks by using positive integer quantities.
                         * It may allow a maximum if a positive (> 0) number
                         *   is specified.
                         */

                        [SerializeField]
                        private int max = 0;

                        public int Max
                        {
                            get { return max; }
                        }

                        public override StackQuantifyingStrategy CreateStackStrategy(object argument)
                        {
                            return new StackIntegerQuantifyingStrategy(this, argument);
                        }
                    }
                }
            }
        }
    }
}
