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

                    /// <summary>
                    ///   Simple rendering strategies provide icon, caption, and quantity to be rendered
                    ///     on compatible inventories. This will be the most common one.
                    /// </summary>
                    public class StackSimpleRenderingStrategy : StackRenderingStrategy
                    {
                        public StackSimpleRenderingStrategy(ItemRenderingStrategy itemStrategy) : base(itemStrategy)
                        {
                        }

                        /// <summary>
                        ///   Dumps icon, caption and quantity into the <paramref name="target"/>.
                        /// </summary>
                        /// <param name="target">Target object on which to dump the render data</param>
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