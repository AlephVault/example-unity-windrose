using System;
using System.Collections.Generic;
using WindRose.Behaviours.Entities.Objects.Strategies;
using WindRose.Types;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace World
        {
            namespace Layers
            {
                namespace Objects
                {
                    namespace ObjectsManagementStrategies
                    {
                        namespace Base
                        {
                            /// <summary>
                            ///   <para>
                            ///     Base management strategies will only check object movement by ensuring
                            ///       their dimensions are never out of the map's bounds.
                            ///   </para>
                            ///   <para>
                            ///     Checking whether movement can be cleared involves checking whether movement
                            ///       is being performed, and its counterpart is
                            ///       <see cref="Entities.Objects.Strategies.Base.BaseObjectStrategy"/>.
                            ///   </para>
                            ///   <seealso cref="ObjectsManagementStrategy"/>
                            ///   <seealso cref="Entities.Objects.Strategies.Base.BaseObjectStrategy"/>
                            /// </summary>
                            public class BaseObjectsManagementStrategy : ObjectsManagementStrategy
                            {
                                public override void AttachedStrategy(ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status)
                                {
                                }

                                public override bool CanAllocateMovement(Dictionary<Type, bool> otherComponentsResults, ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status, Direction direction, bool continuated)
                                {
                                    if (status.Movement != null) return false;

                                    Entities.Objects.MapObject mapObject = strategy.StrategyHolder.Object;

                                    switch (direction)
                                    {
                                        case Direction.LEFT:
                                            return status.X != 0;
                                        case Direction.UP:
                                            return status.Y + mapObject.Height < StrategyHolder.Map.Height;
                                        case Direction.RIGHT:
                                            return status.X + mapObject.Width < StrategyHolder.Map.Width;
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
                                    return typeof(Entities.Objects.Strategies.Base.BaseObjectStrategy);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
