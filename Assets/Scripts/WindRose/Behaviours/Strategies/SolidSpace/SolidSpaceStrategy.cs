using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Strategies
        {
            namespace SolidSpace
            {
                using Types;

                public class SolidSpaceStrategy : Strategy
                {
                    private SolidMask solidMask;
                    private Bitmask blockMask;

                    public SolidSpaceStrategy(StrategyHolder StrategyHolder) : base(StrategyHolder) {}

                    /*****************************************************************************
                     * 
                     * Tilemap initialization will involve creating solid mask and block mask.
                     * However, tilemap computation will only involve block mask, since solid mask
                     *   will not be inferred from tiles but from positionables.
                     * 
                     *****************************************************************************/

                    public override void InitGlobalCellData()
                    {
                        uint width = StrategyHolder.Map.Width;
                        uint height = StrategyHolder.Map.Height;
                        solidMask = new SolidMask(width, height);
                        blockMask = new Bitmask(width, height);
                    }

                    /**
                     * Single-cell computing involves blocking. There are three blocking modes:
                     *   * Blocking
                     *   * Non-Blocking
                     *   * The cell is not a BlockingAware tile, so no change in the blocks value will be done
                     */
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

                    /*****************************************************************************
                     * 
                     * Object attachment.
                     * 
                     *****************************************************************************/

                    public override bool AcceptsObjectStrategy(Objects.Strategies.ObjectStrategy strategy)
                    {
                        return strategy is Objects.Strategies.SolidSpace.SolidSpaceObjectStrategy;
                    }

                    public override void AttachedStratergy(Objects.Strategies.ObjectStrategy strategy, StrategyHolder.Status status)
                    {
                        SolidnessStatus solidness = ((Objects.Strategies.SolidSpace.SolidSpaceObjectStrategy)strategy).Solidness;
                        IncrementBody(strategy, status, solidness);
                    }

                    public override void DetachedStratergy(Objects.Strategies.ObjectStrategy strategy, StrategyHolder.Status status)
                    {
                        SolidnessStatus solidness = ((Objects.Strategies.SolidSpace.SolidSpaceObjectStrategy)strategy).Solidness;
                        DecrementBody(strategy, status, solidness);
                    }

                    /*****************************************************************************
                     * 
                     * Object movement.
                     * 
                     *****************************************************************************/

                    public override bool CanAllocateMovement(Objects.Strategies.ObjectStrategy strategy, StrategyHolder.Status status, Direction direction, bool continuated)
                    {
                        if (status.Movement != null || IsHittingEdge(strategy.StrategyHolder.Positionable, status, direction)) return false;
                        if (IsAdjacencyBlocked(status.X, status.Y, strategy.StrategyHolder.Positionable.Width, strategy.StrategyHolder.Positionable.Height, direction)) return false;
                        SolidnessStatus solidness = ((Objects.Strategies.SolidSpace.SolidSpaceObjectStrategy)strategy).Solidness;
                        return solidness.Traverses() || IsAdjacencyFree(status.X, status.Y, strategy.StrategyHolder.Positionable.Width, strategy.StrategyHolder.Positionable.Height, direction);
                    }

                    public override void DoAllocateMovement(Objects.Strategies.ObjectStrategy strategy, StrategyHolder.Status status, Direction direction, bool continuated, string stage)
                    {
                        switch (stage)
                        {
                            case "AfterMovementAllocation":
                                SolidnessStatus solidness = ((Objects.Strategies.SolidSpace.SolidSpaceObjectStrategy)strategy).Solidness;
                                IncrementAdjacent(strategy, status, solidness);
                                break;
                        }
                    }

                    public override void DoClearMovement(Objects.Strategies.ObjectStrategy strategy, StrategyHolder.Status status, Direction? formerMovement, string stage)
                    {
                        switch(stage)
                        {
                            case "Before":
                                SolidnessStatus solidness = ((Objects.Strategies.SolidSpace.SolidSpaceObjectStrategy)strategy).Solidness;
                                DecrementAdjacent(strategy, status, solidness);
                                break;
                        }
                    }

                    public override void DoConfirmMovement(Objects.Strategies.ObjectStrategy strategy, StrategyHolder.Status status, Direction? formerMovement, string stage)
                    {
                        switch(stage)
                        {
                            case "AfterPositionChange":
                                SolidnessStatus solidness = ((Objects.Strategies.SolidSpace.SolidSpaceObjectStrategy)strategy).Solidness;
                                DecrementOppositeAdjacent(strategy, status, solidness);
                                break;
                        }
                    }

                    public override void DoProcessPropertyUpdate(Objects.Strategies.ObjectStrategy strategy, StrategyHolder.Status status, string property, object oldValue, object newValue)
                    {
                        if (property == "solidness")
                        {
                            ClearMovement(strategy, status);
                            DecrementBody(strategy, status, (SolidnessStatus)oldValue);
                            IncrementBody(strategy, status, (SolidnessStatus)newValue);
                        }
                    }

                    public override void DoTeleport(Objects.Strategies.ObjectStrategy strategy, StrategyHolder.Status status, uint x, uint y, string stage)
                    {
                        SolidnessStatus solidness = ((Objects.Strategies.SolidSpace.SolidSpaceObjectStrategy)strategy).Solidness;
                        switch (stage)
                        {
                            case "Before":
                                ClearMovement(strategy, status);
                                DecrementBody(strategy, status, solidness);
                                break;
                            case "AfterPositionChange":
                                IncrementBody(strategy, status, solidness);
                                break;
                        }
                    }

                    /**
                     * 
                     * Private methods of this particular strategy according to a particular object
                     *   strategy, solidness, and status.
                     * 
                     */

                    private void IncrementBody(Objects.Strategies.ObjectStrategy strategy, StrategyHolder.Status status, SolidnessStatus solidness)
                    {
                        if (solidness.Occupies())
                        {
                            IncrementBody(status.X, status.Y, strategy.StrategyHolder.Positionable.Width, strategy.StrategyHolder.Positionable.Height);
                        }
                        else if (solidness.Carves())
                        {
                            DecrementBody(status.X, status.Y, strategy.StrategyHolder.Positionable.Width, strategy.StrategyHolder.Positionable.Height);
                        }
                    }

                    private void DecrementBody(Objects.Strategies.ObjectStrategy strategy, StrategyHolder.Status status, SolidnessStatus solidness)
                    {
                        if (solidness.Occupies())
                        {
                            DecrementBody(status.X, status.Y, strategy.StrategyHolder.Positionable.Width, strategy.StrategyHolder.Positionable.Height);
                        }
                        else if (solidness.Carves())
                        {
                            IncrementBody(status.X, status.Y, strategy.StrategyHolder.Positionable.Width, strategy.StrategyHolder.Positionable.Height);
                        }
                    }

                    private void IncrementAdjacent(Objects.Strategies.ObjectStrategy strategy, StrategyHolder.Status status, SolidnessStatus solidness)
                    {
                        if (solidness.Occupies())
                        {
                            IncrementAdjacent(status.X, status.Y, strategy.StrategyHolder.Positionable.Width, strategy.StrategyHolder.Positionable.Height, status.Movement);
                        }
                        else if (solidness.Carves())
                        {
                            DecrementAdjacent(status.X, status.Y, strategy.StrategyHolder.Positionable.Width, strategy.StrategyHolder.Positionable.Height, status.Movement);
                        }
                    }

                    private void DecrementAdjacent(Objects.Strategies.ObjectStrategy strategy, StrategyHolder.Status status, SolidnessStatus solidness)
                    {
                        if (solidness.Occupies())
                        {
                            DecrementAdjacent(status.X, status.Y, strategy.StrategyHolder.Positionable.Width, strategy.StrategyHolder.Positionable.Height, status.Movement);
                        }
                        else if (solidness.Carves())
                        {
                            IncrementAdjacent(status.X, status.Y, strategy.StrategyHolder.Positionable.Width, strategy.StrategyHolder.Positionable.Height, status.Movement);
                        }
                    }

                    private void DecrementOppositeAdjacent(Objects.Strategies.ObjectStrategy strategy, StrategyHolder.Status status, SolidnessStatus solidness)
                    {
                        if (solidness.Occupies())
                        {
                            DecrementAdjacent(status.X, status.Y, strategy.StrategyHolder.Positionable.Width, strategy.StrategyHolder.Positionable.Height, status.Movement.Opposite());
                        }
                        else if (solidness.Carves())
                        {
                            IncrementAdjacent(status.X, status.Y, strategy.StrategyHolder.Positionable.Width, strategy.StrategyHolder.Positionable.Height, status.Movement.Opposite());
                        }
                    }

                    /*****************************************************************************
                     * 
                     * Private methods of this particular strategy.
                     * 
                     *****************************************************************************/

                    private void IncrementBody(uint x, uint y, uint width, uint height)
                    {
                        solidMask.IncSquare(x, y, width, height);
                    }

                    private void DecrementBody(uint x, uint y, uint width, uint height)
                    {
                        solidMask.DecSquare(x, y, width, height);
                    }

                    private bool IsHittingEdge(uint x, uint y, uint width, uint height, Direction? direction)
                    {
                        switch (direction)
                        {
                            case Direction.LEFT:
                                return x == 0;
                            case Direction.UP:
                                return y + height == solidMask.height;
                            case Direction.RIGHT:
                                return x + width == solidMask.width;
                            case Direction.DOWN:
                                return y == 0;
                        }
                        return false;
                    }

                    private bool IsAdjacencyBlocked(uint x, uint y, uint width, uint height, Direction? direction)
                    {
                        /** Precondition: IsHittingEdge was already called to this point */
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

                    private bool IsAdjacencyFree(uint x, uint y, uint width, uint height, Direction? direction)
                    {
                        /** Precondition: IsHittingEdge was already called to this point */
                        switch (direction)
                        {
                            case Direction.LEFT:
                                return solidMask.EmptyColumn(x - 1, y, height);
                            case Direction.DOWN:
                                return solidMask.EmptyRow(x, y - 1, width);
                            case Direction.RIGHT:
                                return solidMask.EmptyColumn(x + width, y, height);
                            case Direction.UP:
                                return solidMask.EmptyRow(x, y + height, width);
                            default:
                                return true;
                        }
                    }

                    private void IncrementAdjacent(uint x, uint y, uint width, uint height, Direction? direction)
                    {
                        if (!IsHittingEdge(x, y, width, height, direction))
                        {
                            switch (direction)
                            {
                                case Direction.LEFT:
                                    solidMask.IncColumn(x - 1, y, height);
                                    break;
                                case Direction.DOWN:
                                    solidMask.IncRow(x, y - 1, width);
                                    break;
                                case Direction.RIGHT:
                                    solidMask.IncColumn(x + width, y, height);
                                    break;
                                case Direction.UP:
                                    solidMask.IncRow(x, y + height, width);
                                    break;
                            }
                        }
                    }

                    private void DecrementAdjacent(uint x, uint y, uint width, uint height, Direction? direction)
                    {
                        if (!IsHittingEdge(x, y, width, height, direction))
                        {
                            switch (direction)
                            {
                                case Direction.LEFT:
                                    solidMask.DecColumn(x - 1, y, height);
                                    break;
                                case Direction.DOWN:
                                    solidMask.DecRow(x, y - 1, width);
                                    break;
                                case Direction.RIGHT:
                                    solidMask.DecColumn(x + width, y, height);
                                    break;
                                case Direction.UP:
                                    solidMask.DecRow(x, y + height, width);
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }
}
