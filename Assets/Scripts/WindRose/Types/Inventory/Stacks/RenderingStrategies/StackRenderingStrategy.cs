using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                        public StackRenderingStrategy(ItemRenderingStrategy itemStrategy, Dictionary<string, object> arguments) : base(itemStrategy, arguments)
                        {
                        }
                    }
                }
            }
        }
    }
}