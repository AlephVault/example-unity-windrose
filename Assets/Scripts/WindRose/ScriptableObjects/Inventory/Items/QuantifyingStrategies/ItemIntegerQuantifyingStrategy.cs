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
                    [CreateAssetMenu(fileName = "NewInventoryItemIntegerQuantifyingStrategy", menuName = "Wind Rose/Inventory/Item Strategies/Quantifying/Integer-Stacked", order = 101)]
                    public class ItemIntegerQuantifyingStrategy : ItemQuantifyingStrategy
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

                        private void Awake()
                        {
                            max = Support.Utils.Values.Max(0, max);
                        }

                        public override StackQuantifyingStrategy CreateStackStrategy(object quantity)
                        {
                            return new StackIntegerQuantifyingStrategy(this, quantity);
                        }
                    }
                }
            }
        }
    }
}
