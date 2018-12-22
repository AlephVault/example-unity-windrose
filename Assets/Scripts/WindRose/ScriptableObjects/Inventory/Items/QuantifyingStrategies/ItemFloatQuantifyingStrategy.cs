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
                    [CreateAssetMenu(fileName = "NewInventoryItem", menuName = "Wind Rose/Inventory/Item Strategies/Quantifying/Float-Stacked", order = 101)]
                    public class ItemFloatQuantifyingStrategy : ItemQuantifyingStrategy
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
