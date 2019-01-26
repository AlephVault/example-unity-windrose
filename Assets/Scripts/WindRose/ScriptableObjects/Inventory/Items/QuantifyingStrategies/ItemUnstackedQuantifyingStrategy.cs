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
                    /// <summary>
                    ///   Unstacked strategies do not make a stack of items.
                    ///   This actually means: they only make stacks of ONE item.
                    /// </summary>
                    [CreateAssetMenu(fileName = "NewInventoryItemUnstackedQuantifyingStrategy", menuName = "Wind Rose/Inventory/Item Strategies/Quantifying/Unstacked", order = 101)]
                    public class ItemUnstackedQuantifyingStrategy : ItemQuantifyingStrategy
                    {
                        /// <summary>
                        ///   Instantiates an unstacked quantifying stack strategy.
                        /// </summary>
                        /// <returns>An unstacked quantifying stack strategy</returns>
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
