using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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
                    ///   Static rendering strategies provide nothing as render data.
                    ///     They are intended to be used with variants of the static
                    ///     rendering management strategies.
                    /// </summary>
                    public class StackStaticRenderingStrategy : StackRenderingStrategy
                    {
                        public StackStaticRenderingStrategy(ItemRenderingStrategy itemStrategy) : base(itemStrategy) {}

                        /// <summary>
                        ///   Adds nothing to <paramref name="target"/>. This, because
                        ///     nothing conditions what is specified in the item's data
                        ///     (since this element is conceived as static).
                        /// </summary>
                        /// <param name="target">Target object on which to dump the render data</param>
                        public override void DumpRenderingData(Dictionary<string, object> target) {}
                    }
                }
            }
        }
    }
}