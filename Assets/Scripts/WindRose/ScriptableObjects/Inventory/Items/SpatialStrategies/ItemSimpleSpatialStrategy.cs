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
                    /// <summary>
                    ///   Simple spatial strategies do not add anything on top. They
                    ///     are intended to occupy a single slot in a sequential
                    ///     inventory spatial management strategy.
                    /// </summary>
                    [CreateAssetMenu(fileName = "NewInventoryItemSimpleSpatialStrategy", menuName = "Wind Rose/Inventory/Item Strategies/Spatial/Simple", order = 101)]
                    public class ItemSimpleSpatialStrategy : ItemSpatialStrategy
                    {
                    }
                }
            }
        }
    }
}
