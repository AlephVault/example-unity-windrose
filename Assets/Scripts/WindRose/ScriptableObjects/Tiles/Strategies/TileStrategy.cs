using UnityEngine;

namespace WindRose
{
    namespace ScriptableObjects
    {
        namespace Tiles
        {
            namespace Strategies
            {
                public abstract class TileStrategy : ScriptableObject
                {
                    /**
                     * This is just a marker class. Just a kind of data bundle
                     *   for the tile. It will not execute particular logic
                     *   but it may provide methods to be executed by map
                     *   strategies.
                     */
                }
            }
        }
    }
}
