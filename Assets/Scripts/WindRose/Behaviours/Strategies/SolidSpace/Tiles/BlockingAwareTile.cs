﻿using UnityEngine;
using UnityEngine.Tilemaps;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Strategies
        {
            namespace SolidSpace
            {
                namespace Tiles
                {
                    [CreateAssetMenu(fileName = "NewBlockingTile", menuName = "Wind Rose/Tiles/Blocking Tile", order = 201)]
                    public class BlockingAwareTile : Tile, IBlockingAwareTile
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
    }
}
