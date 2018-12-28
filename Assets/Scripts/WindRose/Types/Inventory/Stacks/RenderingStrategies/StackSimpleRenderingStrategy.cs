using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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

                    public class StackSimpleRenderingStrategy : StackRenderingStrategy
                    {
                        /**
                         * This stack strategy provides, from its item, the data to be rendered: name and caption.
                         * From the stack, it will retrieve the quantity to be rendered (which may be int, float, or null).
                         */

                        public StackSimpleRenderingStrategy(ItemRenderingStrategy itemStrategy) : base(itemStrategy)
                        {
                        }

                        public override void DumpRenderingData(Dictionary<string, object> target)
                        {
                            ItemSimpleRenderingStrategy strategy = ((ItemSimpleRenderingStrategy)ItemStrategy);
                            target["icon"] = strategy.Icon;
                            target["caption"] = strategy.Caption;
                            target["quantity"] = Stack.QuantifyingStrategy.Quantity;
                        }
                    }
                }
            }
        }
    }
}