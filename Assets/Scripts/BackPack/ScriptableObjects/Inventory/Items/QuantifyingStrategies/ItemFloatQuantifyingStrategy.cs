using UnityEngine;
using AlephVault.Unity.Support.Utils;

namespace BackPack
{
	using Types.Inventory.Stacks.QuantifyingStrategies;

    namespace ScriptableObjects
    {
        namespace Inventory
        {
            namespace Items
            {
                namespace QuantifyingStrategies
                {
                    /// <summary>
                    ///   <para>
                    ///     Float strategies are not common. They make
                    ///       stacks in float quantities.
                    ///   </para>
                    ///   <para>
                    ///     Quite often a maximum amount is specified to
                    ///       limit the stacks.
                    ///   </para>1
                    /// </summary>
                    [CreateAssetMenu(fileName = "NewInventoryItemFloatQuantifyingStrategy", menuName = "Wind Rose/Inventory/Item Strategies/Quantifying/Float-Stacked", order = 101)]
                    public class ItemFloatQuantifyingStrategy : ItemQuantifyingStrategy
                    {
                        /// <summary>
                        ///   The maximum amount to allow in stacks.
                        /// </summary>
                        [SerializeField]
                        private float max = 0;

                        /// <summary>
                        ///   See <see cref="max"/>.
                        /// </summary>
                        public float Max
                        {
                            get { return max; }
                        }

                        private void Awake()
                        {
                            max = Values.Max(0f, max);
                        }

                        /// <summary>
                        ///   Instantiates a float quantifying stack strategy.
                        /// </summary>
                        /// <returns>A float quantifying stack strategy</returns>
                        public override StackQuantifyingStrategy CreateStackStrategy(object quantity)
                        {
                            return new StackFloatQuantifyingStrategy(this, quantity);
                        }
                    }
                }
            }
        }
    }
}
