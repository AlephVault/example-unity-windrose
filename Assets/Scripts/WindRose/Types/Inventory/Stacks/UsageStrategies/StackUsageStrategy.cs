using System;

namespace WindRose
{
    namespace Types
    {
        namespace Inventory
        {
            namespace Stacks
            {
                namespace UsageStrategies
                {
                    using ScriptableObjects.Inventory.Items.UsageStrategies;

                    /// <summary>
                    ///   Stack usage strategoes are related to Item usage strategies
                    ///     and MAY hold dynamic data related to them. They can also
                    ///     change states, merge, compare by equality, export and import.
                    /// </summary>
                    public abstract class StackUsageStrategy : StackStrategy<ItemUsageStrategy>
                    {
                        /// <summary>
                        ///   This stack strategy is related to an ItemUsageStrategy.
                        /// </summary>
                        public StackUsageStrategy(ItemUsageStrategy itemStrategy) : base(itemStrategy)
                        {
                        }

                        /// <summary>
                        ///   Clones a usage strategy. Useful for cloning or splitting stacks.
                        /// </summary>
                        public StackUsageStrategy Clone()
                        {
                            StackUsageStrategy strategy = ItemStrategy.CreateStackStrategy();
                            strategy.Import(Export());
                            return strategy;
                        }

                        /// <summary>
                        ///   Imports its data from a source object.
                        /// </summary>
                        /// <param name="source">The source to import from</param>
                        public virtual void Import(object source)
                        {
                        }

                        /// <summary>
                        ///   Exports the data of this strategy.
                        /// </summary>
                        /// <returns>A data object, suitable to import it by the same strategy</returns>
                        public virtual object Export()
                        {
                            return null;
                        }

                        /// <summary>
                        ///   <para>
                        ///     Tries to interpolate two strategies by also considering quantities. Interpolating means computing
                        ///       properties like <c>p1 = currentQuantity * p1 + addedQuantity * p2</c> in concept.
                        ///   </para>
                        ///   <para>
                        ///     There may be cases where the current strategy and the one to interpolate are not compatible (a
                        ///       base case involves failing in <see cref="Equals(StackUsageStrategy)"/>). In such cases the
                        ///       appropriate return value is <c>null</c>. Otherwise, the appropriate return value is a function
                        ///       (delegate) that performs the interpolation (such function is totally user-defined, or at least
                        ///       usage-strategy-developer-defined, but IT MUST NOT PROVOKE A SIDE-EFFECT ON THE otherStrategy).
                        ///   </para>
                        /// </summary>
                        /// <param name="otherStrategy">The strategy to interpolate against</param>
                        public virtual Action Interpolate(StackUsageStrategy otherStrategy, object currentQuantity, object addedQuantity)
                        {
                            return (Equals(otherStrategy)) ? delegate (){} : (Action)null;
                        }

                        /// <summary>
                        ///   Tells whether this particular strategy equals another particular strategy. This will start often by
                        ///     checking types, and -if available- will also check the data in both strategies.
                        /// </summary>
                        /// <param name="otherStrategy">The strategy to compare against</param>
                        /// <returns>Whether they can be considered equal or not</returns>
                        public abstract bool Equals(StackUsageStrategy otherStrategy);
                    }
                }
            }
        }
    }
}
