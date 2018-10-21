﻿using System;
using System.Collections.Generic;
using UnityEngine;
using WindRose.Behaviours.Objects.Strategies;
using WindRose.Types;

namespace WindRose
{
    namespace Behaviours
    {
        namespace World
        {
            namespace Strategies
            {
                namespace Base
                {
                    /**
                     * This class allows telling which cells are blocking. Objects are not allowed to
                     *   traverse blocking cells. Blocked positions will be mantained in a bitmask
                     *   and determined by the topmost blocking (or unblocking) tilemap's cell for
                     *   a certain (x, y) pair.
                     */
                    [RequireComponent(typeof(BaseStrategy))]
                    public class LayoutStrategy : Strategy
                    {
                        private Bitmask blockMask;

                        private bool IsAdjacencyBlocked(uint x, uint y, uint width, uint height, Direction? direction)
                        {
                            switch (direction)
                            {
                                case Direction.LEFT:
                                    return blockMask.GetColumn(x - 1, y, y + height - 1, Bitmask.CheckType.ANY_BLOCKED);
                                case Direction.DOWN:
                                    return blockMask.GetRow(x, x + width - 1, y - 1, Bitmask.CheckType.ANY_BLOCKED);
                                case Direction.RIGHT:
                                    return blockMask.GetColumn(x + width, y, y + height - 1, Bitmask.CheckType.ANY_BLOCKED);
                                case Direction.UP:
                                    return blockMask.GetRow(x, x + width - 1, y + height, Bitmask.CheckType.ANY_BLOCKED);
                                default:
                                    return true;
                            }
                        }

                        public override bool CanAllocateMovement(Dictionary<Type, bool> otherComponentsResults, ObjectStrategy strategy, StrategyHolder.Status status, Direction direction, bool continuated)
                        {
                            // First follows what the BaseStrategy tells
                            if (!otherComponentsResults[typeof(BaseStrategy)]) return false;

                            // Then check for cells being blocked
                            return !IsAdjacencyBlocked(status.X, status.Y, strategy.StrategyHolder.Positionable.Width, strategy.StrategyHolder.Positionable.Height, direction);
                        }

                        public override bool CanClearMovement(Dictionary<Type, bool> otherComponentsResults, ObjectStrategy strategy, StrategyHolder.Status status)
                        {
                            // Just follows what the BaseStrategy tells
                            return otherComponentsResults[typeof(BaseStrategy)];
                        }

                        public override void ComputeCellData(uint x, uint y)
                        {
                            bool blocks = false;
                            StrategyHolder.ForEachTilemap(delegate (UnityEngine.Tilemaps.Tilemap tilemap) {
                                UnityEngine.Tilemaps.TileBase tile = tilemap.GetTile(new Vector3Int((int)x, (int)y, 0));
                                if (tile is Tiles.IBlockingAwareTile)
                                {
                                    blocks = ((Tiles.IBlockingAwareTile)tile).Blocks();
                                }
                                return false;
                            });
                            blockMask.SetCell(x, y, blocks);
                        }

                        public override void InitGlobalCellsData()
                        {
                            uint width = StrategyHolder.Map.Width;
                            uint height = StrategyHolder.Map.Height;
                            blockMask = new Bitmask(width, height);
                        }

                        protected override Type GetCounterpartType()
                        {
                            return typeof(Objects.Strategies.Base.LayoutObjectStrategy);
                        }
                    }
                }
            }
        }
    }
}
