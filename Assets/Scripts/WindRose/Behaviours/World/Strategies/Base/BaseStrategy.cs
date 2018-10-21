using System;
using System.Collections.Generic;
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
                     * This base strategy is unavoidable as it offers essential logic that is needed
                     *   in order to even prevent RuntimeErrors. This class disallows starting movement
                     *   if a movement is in progress OR if an edge is being hit.
                     */
                    public class BaseStrategy : Strategy
                    {
                        public override void AttachedStrategy(ObjectStrategy strategy, StrategyHolder.Status status)
                        {
                        }

                        public override bool CanAllocateMovement(Dictionary<Type, bool> otherComponentsResults, ObjectStrategy strategy, StrategyHolder.Status status, Direction direction, bool continuated)
                        {
                            if (status.Movement != null) return false;

                            Objects.Positionable positionable = strategy.StrategyHolder.Positionable;

                            switch (direction)
                            {
                                case Direction.LEFT:
                                    return status.X != 0;
                                case Direction.UP:
                                    return status.Y + positionable.Height != StrategyHolder.Map.Height;
                                case Direction.RIGHT:
                                    return status.X + positionable.Width != StrategyHolder.Map.Width;
                                case Direction.DOWN:
                                    return status.Y != 0;
                            }
                            return true;
                        }

                        public override bool CanClearMovement(Dictionary<Type, bool> otherComponentsResults, ObjectStrategy strategy, StrategyHolder.Status status)
                        {
                            return status.Movement != null;
                        }

                        public override void ComputeCellData(uint x, uint y)
                        {
                        }

                        public override void DetachedStrategy(ObjectStrategy strategy, StrategyHolder.Status status)
                        {
                        }

                        public override void DoAllocateMovement(ObjectStrategy strategy, StrategyHolder.Status status, Direction direction, bool continuated, string stage)
                        {
                        }

                        public override void DoClearMovement(ObjectStrategy strategy, StrategyHolder.Status status, Direction? formerMovement, string stage)
                        {
                        }

                        public override void DoConfirmMovement(ObjectStrategy strategy, StrategyHolder.Status status, Direction? formerMovement, string stage)
                        {
                        }

                        public override void DoProcessPropertyUpdate(ObjectStrategy strategy, StrategyHolder.Status status, string property, object oldValue, object newValue)
                        {
                        }

                        public override void DoTeleport(ObjectStrategy strategy, StrategyHolder.Status status, uint x, uint y, string stage)
                        {
                        }

                        public override void InitGlobalCellsData()
                        {
                        }

                        protected override Type GetCounterpartType()
                        {
                            return typeof(Objects.Strategies.Base.BaseObjectStrategy);
                        }
                    }
                }
            }
        }
    }
}
