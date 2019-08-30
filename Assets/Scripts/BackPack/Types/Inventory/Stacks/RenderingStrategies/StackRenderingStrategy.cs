using System.Collections.Generic;

namespace BackPack
{
    namespace Types
    {
        namespace Inventory
        {
            namespace Stacks
            {
                namespace RenderingStrategies
                {
                    using ScriptableObjects.Inventory.Items.RenderingStrategies;

                    /// <summary>
                    ///   Stack rendering strategies are related to item rendering strategies and
                    ///     they only serve to provide rendering data (inside an existing dictionary
                    ///     that will be accumulating the render data).
                    /// </summary>
                    public abstract class StackRenderingStrategy : StackStrategy<ItemRenderingStrategy>
                    {
                        /**
                         * This stack strategy is related to an ItemRenderingStrategy.
                         */
                        public StackRenderingStrategy(ItemRenderingStrategy itemStrategy) : base(itemStrategy)
                        {
                        }

                        /// <summary>
                        ///   Dumps data to render in that dictionary. It also may read data from that
                        ///     dictionary before dumping anything.
                        /// </summary>
                        /// <param name="target">The data bulk to read/dump data from/into</param>
                        public abstract void DumpRenderingData(Dictionary<string, object> target);

                        /// <summary>
                        ///   Clones a rendering strategy.Useful for cloning or splitting stacks.
                        ///   No arguments are needed for rendering strategies.
                        /// </summary>
                        public StackRenderingStrategy Clone()
                        {
                            return ItemStrategy.CreateStackStrategy();
                        }
                    }
                }
            }
        }
    }
}