using UnityEngine;

namespace BackPack
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
                    ///   1D Indexed spatial strategies do not add anything on top.
                    ///     They are intended to occupy a single slot in a sequential
                    ///     inventory spatial management strategy.
                    /// </summary>
                    [CreateAssetMenu(fileName = "NewInventoryItem1DIndexedSpatialStrategy", menuName = "Wind Rose/Inventory/Item Strategies/Spatial/1D Indexed", order = 101)]
                    public class Item1DIndexedSpatialStrategy : ItemSpatialStrategy
                    {
                    }
                }
            }
        }
    }
}
