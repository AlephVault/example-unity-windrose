using UnityEngine;

namespace WindRose
{
    namespace ScriptableObjects
    {
        namespace Inventory
        {
            namespace Items
            {
                namespace SpatialStrategies
                {
                    [CreateAssetMenu(fileName = "NewInventoryItemSimpleSpatialStrategy", menuName = "Wind Rose/Inventory/Item Strategies/Spatial/Simple", order = 101)]
                    public class ItemSimpleSpatialStrategy : ItemSpatialStrategy
                    {
                        /**
                         * This is a simple spatial strategy that indexes
                         *   its content. Most games using inventory make
                         *   use of this approach: each item stack will
                         *   be contained in one single box.
                         * 
                         * This strategy needs no additional data (other
                         *   strategies may involve weight or dimensions).
                         */
                    }
                }
            }
        }
    }
}
