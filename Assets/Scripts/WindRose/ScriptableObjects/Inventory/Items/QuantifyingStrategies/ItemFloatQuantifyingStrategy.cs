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
                    class ItemFloatQuantifyingStrategy : ItemQuantifyingStrategy
                    {
                        /**
                         * Makes stacks by using positive float quantities.
                         * It may allow a maximum if a positive (> 0) number
                         *   is specified.
                         */

                        [SerializeField]
                        private float max = 0;

                        public float Max
                        {
                            get { return max; }
                        }

                        public override StackQuantifyingStrategy CreateStackStrategy(object argument)
                        {
                            return new StackFloatQuantifyingStrategy(this, argument);
                        }
                    }
                }
            }
        }
    }
}
