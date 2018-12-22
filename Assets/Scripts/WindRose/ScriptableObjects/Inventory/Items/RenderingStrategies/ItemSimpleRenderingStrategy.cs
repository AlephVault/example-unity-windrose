using UnityEngine;

namespace WindRose
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

                    [CreateAssetMenu(fileName = "NewInventoryItemSimpleRenderingStrategy", menuName = "Wind Rose/Inventory/Item Strategies/Rendering/Simple", order = 101)]
                    [RequireSpatialStrategy(typeof(SpatialStrategies.ItemSimpleSpatialStrategy))]
                    class ItemSimpleRenderingStrategy : ItemRenderingStrategy
                    {
                        /**
                         * This is a simple strategy for rendering items. Simple strategies are the
                         *   most common among games when dealing with inventories: They will have
                         *   a caption and an icon.
                         * 
                         * For this to work, this strategy will be compatible with Simple Spatial
                         *   strategies (usage/data/quantifying strategies have no restriction).
                         */

                        [SerializeField]
                        private Sprite icon;

                        [SerializeField]
                        private string caption;

                        public Sprite Icon
                        {
                            get { return icon; }
                        }

                        public string Caption
                        {
                            get { return caption; }
                        }

                        public override StackRenderingStrategy CreateStackStrategy(object argument)
                        {
                            return new StackSimpleRenderingStrategy(this, argument);
                        }
                    }
                }
            }
        }
    }
}
