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
                    ///   Simple rendering strategies are the most common in games
                    ///     dealing with inventories: They consider an image, a caption
                    ///     and related data (like quantities) to be rendered as a
                    ///     single item in a single slot.
                    /// </summary>
                    [CreateAssetMenu(fileName = "NewInventoryItemSimpleRenderingStrategy", menuName = "Wind Rose/Inventory/Item Strategies/Rendering/Simple", order = 101)]
                    [RequireSpatialStrategy(typeof(SpatialStrategies.ItemSimpleSpatialStrategy))]
                    public class ItemSimpleRenderingStrategy : ItemRenderingStrategy
                    {
                        /// <summary>
                        ///   The icon to render.
                        /// </summary>
                        [SerializeField]
                        private Sprite icon;

                        /// <summary>
                        ///   The caption to show.
                        /// </summary>
                        [SerializeField]
                        private string caption;

                        /// <summary>
                        ///   See <see cref="icon"/>.
                        /// </summary>
                        public Sprite Icon
                        {
                            get { return icon; }
                        }

                        /// <summary>
                        ///   See <see cref="caption"/>.
                        /// </summary>
                        public string Caption
                        {
                            get { return caption; }
                        }

                        /// <summary>
                        ///   Instantiates a simple rendering stack strategy.
                        /// </summary>
                        /// <returns>A simple rendering stack strategy</returns>
                        public override StackRenderingStrategy CreateStackStrategy()
                        {
                            return new StackSimpleRenderingStrategy(this);
                        }
                    }
                }
            }
        }
    }
}
