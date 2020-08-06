using System;

namespace BackPack
{
	using ScriptableObjects.Inventory.Items.UsageStrategies;

    namespace Types
    {
        namespace Inventory
        {
            namespace Stacks
            {
                namespace UsageStrategies
                {
                    /// <summary>
                    ///   Stacks with this strategy do nothing when attempted to use them. In the same way,
                    ///     interpolation will be trivial and always succeed.
                    /// </summary>
                    public class StackNullUsageStrategy : StackUsageStrategy
                    {
                        public StackNullUsageStrategy(ItemUsageStrategy itemStrategy) : base(itemStrategy)
                        {
                        }

                        /// <summary>
                        ///   Returns whether the other strategy is also a null usage strategy or not.
                        /// </summary>
                        /// <param name="otherStrategy">The strategy to compare against</param>
                        /// <returns>Whether they can be considered equal or not</returns>
                        public override bool Equals(StackUsageStrategy otherStrategy)
                        {
                            // Yes: an exact class check!
                            return otherStrategy.GetType() == typeof(StackNullUsageStrategy);
                        }
                    }
                }
            }
        }
    }
}
