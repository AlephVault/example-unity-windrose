using UnityEngine;

namespace BackPack
{
    namespace ScriptableObjects
    {
        namespace Inventory
        {
            namespace Items
            {
                namespace RenderingStrategies
                {
                    using Types.Inventory.Stacks.RenderingStrategies;

                    /// <summary>
                    ///   Static rendering strategies are those to be
                    ///     used with the static rendering management
                    ///     strategy, where the item has all the data
                    ///     on itself to do the rendering.
                    /// </summary>
                    public class ItemStaticRenderingStrategy : ItemRenderingStrategy
                    {
                        /// <summary>
                        ///   Instantiates a static rendering stack strategy.
                        /// </summary>
                        /// <returns>A static rendering stack strategy</returns>
                        public override StackRenderingStrategy CreateStackStrategy()
                        {
                            return new StackStaticRenderingStrategy(this);
                        }
                    }
                }
            }
        }
    }
}
