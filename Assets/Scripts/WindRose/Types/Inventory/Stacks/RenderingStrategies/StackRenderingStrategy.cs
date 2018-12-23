using System.Collections.Generic;

namespace WindRose
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

                    public abstract class StackRenderingStrategy : StackStrategy<ItemRenderingStrategy>
                    {
                        /**
                         * This stack strategy is related to an ItemRenderingStrategy.
                         */
                        public StackRenderingStrategy(ItemRenderingStrategy itemStrategy) : base(itemStrategy)
                        {
                        }

                        public abstract void DumpRenderingData(Dictionary<string, object> target);

                        /**
                         * Clones a rendering strategy. Useful for cloning or splitting stacks.
                         * No arguments are needed for rendering strategies.
                         */
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