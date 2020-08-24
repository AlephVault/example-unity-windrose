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
                    /// <summary>
                    ///   Icon/Text rendering strategies are the most common in games
                    ///     dealing with inventories: They consider an image, a caption
                    ///     to be rendered as a single item in a single slot (according
                    ///     to the quantity).
                    /// </summary>
                    [CreateAssetMenu(fileName = "NewInventoryItemIconTextRenderingStrategy", menuName = "Wind Rose/Inventory/Item Strategies/Rendering/Icon & Text", order = 101)]
                    public class ItemIconTextRenderingStrategy : ItemStaticRenderingStrategy
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
                    }
                }
            }
        }
    }
}
