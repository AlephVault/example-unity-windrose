﻿using UnityEngine;
using UnityEngine.Tilemaps;

namespace WindRose
{
    namespace Types
    {
        namespace Tilemaps
        {
            [CreateAssetMenu(fileName = "NewBlockingAnimatedTile", menuName = "Wind Rose/Tiles/Blocking Animated Tile", order = 202)]
            public class BlockingAwareAnimatedTile : AnimatedTile, IBlockingAwareTile
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
