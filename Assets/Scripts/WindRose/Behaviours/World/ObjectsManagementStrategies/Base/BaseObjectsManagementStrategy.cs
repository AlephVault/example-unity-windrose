using System;
using System.Collections.Generic;
using WindRose.Behaviours.Objects.Strategies;
using WindRose.Types;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace World
        {
            namespace ObjectsManagementStrategies
            {
                namespace Base
                {
                    /**
                     * This base strategy is unavoidable as it offers essential logic that is needed
                     *   in order to even prevent RuntimeErrors. This class disallows starting movement
                     *   if a movement is in progress OR if an edge is being hit.
                     */
                    public class BaseObjectsManagementStrategy : ObjectsManagementStrategy
                    {
                        public override void AttachedStrategy(ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status)
                        {
                        }

                        public override bool CanAllocateMovement(Dictionary<Type, bool> otherComponentsResults, ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status, Direction direction, bool continuated)
                        {
                            if (status.Movement != null) return false;

                            Objects.Positionable positionable = strategy.StrategyHolder.Positionable;

                            switch (direction)
                            {
                                case Direction.LEFT:
                                    return status.X != 0;
                                case Direction.UP:
                                    return status.Y + positionable.Height < StrategyHolder.Map.Height;
                                case Direction.RIGHT:
                                    return status.X + positionable.Width < StrategyHolder.Map.Width;
                                case Direction.DOWN:
                                    return status.Y != 0;
                            }
                            return true;
                        }

                        public override bool CanClearMovement(Dictionary<Type, bool> otherComponentsResults, ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status)
                        {
                            return status.Movement != null;
                        }

                        public override void ComputeCellData(uint x, uint y)
                        {
                        }

                        public override void DetachedStrategy(ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status)
                        {
                        }

                        public override void DoAllocateMovement(ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status, Direction direction, bool continuated, string stage)
                        {
                        }

                        public override void DoClearMovement(ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status, Direction? formerMovement, string stage)
                        {
                        }

                        public override void DoConfirmMovement(ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status, Direction? formerMovement, string stage)
                        {
                        }

                        public override void DoProcessPropertyUpdate(ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status, string property, object oldValue, object newValue)
                        {
                        }

                        public override void DoTeleport(ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status, uint x, uint y, string stage)
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
