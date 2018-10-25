using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace World
        {
            namespace ObjectsManagementStrategies
            {
                namespace Solidness
                {
                    using Types;

                    [RequireComponent(typeof(Base.BaseObjectsManagementStrategy))]
                    public class SolidnessObjectsManagementStrategy : ObjectsManagementStrategy
                    {
                        private SolidMask solidMask;

                        protected override Type GetCounterpartType()
                        {
                            return typeof(Objects.Strategies.Solidness.SolidnessObjectStrategy);
                        }

                        /*****************************************************************************
                         * 
                         * Tilemap initialization will involve creating solid mask and block mask.
                         * However, tilemap computation will only involve block mask, since solid mask
                         *   will not be inferred from tiles but from positionables.
                         * 
                         *****************************************************************************/

                        public override void InitGlobalCellsData()
                        {
                            uint width = StrategyHolder.Map.Width;
                            uint height = StrategyHolder.Map.Height;
                            solidMask = new SolidMask(width, height);
                        }

                        /*****************************************************************************
                         * 
                         * Object attachment.
                         * 
                         *****************************************************************************/

                        public override void AttachedStrategy(Objects.Strategies.ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status)
                        {
                            SolidnessStatus solidness = ((Objects.Strategies.Solidness.SolidnessObjectStrategy)strategy).Solidness;
                            IncrementBody(strategy, status, solidness);
                        }

                        public override void DetachedStrategy(Objects.Strategies.ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status)
                        {
                            SolidnessStatus solidness = ((Objects.Strategies.Solidness.SolidnessObjectStrategy)strategy).Solidness;
                            DecrementBody(strategy, status, solidness);
                        }

                        /*****************************************************************************
                         * 
                         * Object movement.
                         * 
                         *****************************************************************************/

                        public override bool CanAllocateMovement(Dictionary<Type, bool> otherComponentsResults, Objects.Strategies.ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status, Direction direction, bool continuated)
                        {
                            if (!otherComponentsResults[typeof(Base.BaseObjectsManagementStrategy)]) return false;
                            SolidnessStatus solidness = ((Objects.Strategies.Solidness.SolidnessObjectStrategy)strategy).Solidness;
                            return solidness.Traverses() || IsAdjacencyFree(status.X, status.Y, strategy.StrategyHolder.Positionable.Width, strategy.StrategyHolder.Positionable.Height, direction);
                        }

                        public override bool CanClearMovement(Dictionary<Type, bool> otherComponentsResults, Objects.Strategies.ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status)
                        {
                            // Just follows what the BaseStrategy tells
                            return otherComponentsResults[typeof(Base.BaseObjectsManagementStrategy)];
                        }

                        public override void DoAllocateMovement(Objects.Strategies.ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status, Direction direction, bool continuated, string stage)
                        {
                            switch (stage)
                            {
                                case "AfterMovementAllocation":
                                    SolidnessStatus solidness = ((Objects.Strategies.Solidness.SolidnessObjectStrategy)strategy).Solidness;
                                    IncrementAdjacent(strategy, status, solidness);
                                    break;
                            }
                        }

                        public override void DoClearMovement(Objects.Strategies.ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status, Direction? formerMovement, string stage)
                        {
                            switch (stage)
                            {
                                case "Before":
                                    SolidnessStatus solidness = ((Objects.Strategies.Solidness.SolidnessObjectStrategy)strategy).Solidness;
                                    DecrementAdjacent(strategy, status, solidness);
                                    break;
                            }
                        }

                        public override void DoConfirmMovement(Objects.Strategies.ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status, Direction? formerMovement, string stage)
                        {
                            switch (stage)
                            {
                                case "AfterPositionChange":
                                    SolidnessStatus solidness = ((Objects.Strategies.Solidness.SolidnessObjectStrategy)strategy).Solidness;
                                    DecrementOppositeAdjacent(strategy, status, solidness);
                                    break;
                            }
                        }

                        public override void DoProcessPropertyUpdate(Objects.Strategies.ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status, string property, object oldValue, object newValue)
                        {
                            if (property == "solidness")
                            {
                                StrategyHolder.MovementCancel(strategy.StrategyHolder);
                                DecrementBody(strategy, status, (SolidnessStatus)oldValue);
                                IncrementBody(strategy, status, (SolidnessStatus)newValue);
                            }
                        }

                        public override void DoTeleport(Objects.Strategies.ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status, uint x, uint y, string stage)
                        {
                            SolidnessStatus solidness = ((Objects.Strategies.Solidness.SolidnessObjectStrategy)strategy).Solidness;
                            switch (stage)
                            {
                                case "Before":
                                    StrategyHolder.MovementCancel(strategy.StrategyHolder);
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

                        private void IncrementBody(Objects.Strategies.ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status, SolidnessStatus solidness)
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

                        private void DecrementBody(Objects.Strategies.ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status, SolidnessStatus solidness)
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

                        private void IncrementAdjacent(Objects.Strategies.ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status, SolidnessStatus solidness)
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

                        private void DecrementAdjacent(Objects.Strategies.ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status, SolidnessStatus solidness)
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

                        private void DecrementOppositeAdjacent(Objects.Strategies.ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status, SolidnessStatus solidness)
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
}
