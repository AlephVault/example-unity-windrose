using UnityEngine;
using UnityEngine.Tilemaps;

namespace WindRose
{
    namespace Types
    {
        namespace Tilemaps
        {
            [CreateAssetMenu(fileName = "NewBlockingRandomTile", menuName = "Wind Rose/Tiles/Blocking Random Tile", order = 203)]
            public class BlockingAwareRandomTile : RandomTile, IBlockingAwareTile
            {
                [SerializeField]
                private bool blocks = true;

                public bool Blocks()
                {
                    return blocks;
                }
            }
        }
    }
}
