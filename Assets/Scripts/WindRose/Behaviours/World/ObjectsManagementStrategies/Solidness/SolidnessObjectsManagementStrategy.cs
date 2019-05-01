﻿using System;
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

                    /// <summary>
                    ///   <para>
                    ///     Solidness management strategies involve how the objects occupy
                    ///       the cells in the map. This strategy will add the data of the
                    ///       objects solidness inside a <see cref="SolidMask"/>, which will
                    ///       be used to tell whether fully solid objects can walk through
                    ///       their cells or not.
                    ///   </para>
                    ///   <para>
                    ///     Its counterpart is <see cref="Entities.Objects.Strategies.Solidness.SolidnessObjectStrategy"/>.
                    ///   </para> 
                    /// </summary>
                    [RequireComponent(typeof(Base.BaseObjectsManagementStrategy))]
                    public class SolidnessObjectsManagementStrategy : ObjectsManagementStrategy
                    {
                        private SolidMask solidMask;

                        /// <summary>
                        ///   The step type to use when processing objects' movements.
                        /// </summary>
                        [SerializeField]
                        private StepType stepType = StepType.Safe;

                        protected override Type GetCounterpartType()
                        {
                            return typeof(Entities.Objects.Strategies.Solidness.SolidnessObjectStrategy);
                        }

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

                        /// <summary>
                        ///   When the object is attached to the map, its solidness will be added to the
                        ///     cell(s) it occupies.
                        /// </summary>
                        public override void AttachedStrategy(Entities.Objects.Strategies.ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status)
                        {
                            Entities.Objects.Strategies.Solidness.SolidnessObjectStrategy solidnessStrategy = ((Entities.Objects.Strategies.Solidness.SolidnessObjectStrategy)strategy);
                            SolidnessStatus solidness = solidnessStrategy.Solidness;
                            IncrementBody(solidnessStrategy, status, solidness);
                        }

                        /// <summary>
                        ///   When the object is detached from the map, its solidness will be cleared from the
                        ///     cell(s) it occupies.
                        /// </summary>
                        public override void DetachedStrategy(Entities.Objects.Strategies.ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status)
                        {
                            Entities.Objects.Strategies.Solidness.SolidnessObjectStrategy solidnessStrategy = ((Entities.Objects.Strategies.Solidness.SolidnessObjectStrategy)strategy);
                            SolidnessStatus solidness = solidnessStrategy.Solidness;
                            DecrementBody(solidnessStrategy, status, solidness);
                        }

                        /*****************************************************************************
                         * 
                         * Object movement.
                         * 
                         *****************************************************************************/

                        /// <summary>
                        ///   <para>
                        ///     Allowing movement in certain direction involves checking for solidness according
                        ///       to the objects occupying or not such cells and the solidness they add, and also
                        ///       counting the current object's solidness. Additionally, the check value coming from
                        ///       <see cref="Base.BaseObjectsManagementStrategy.CanAllocateMovement(Dictionary{Type, bool}, Objects.Strategies.ObjectStrategy, ObjectsManagementStrategyHolder.Status, Direction, bool)"/>
                        ///       is also relevant here since we don't want to allow movement outside the map bounds.
                        ///   </para>
                        ///   <para>
                        ///     See <see cref="ObjectsManagementStrategy.CanAllocateMovement(Dictionary{Type, bool}, Objects.Strategies.ObjectStrategy, ObjectsManagementStrategyHolder.Status, Direction, bool)"/>
                        ///       for more information on this method's signature and intention.
                        ///   </para>
                        /// </summary>
                        public override bool CanAllocateMovement(Dictionary<Type, bool> otherComponentsResults, Entities.Objects.Strategies.ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status, Direction direction, bool continuated)
                        {
                            if (!otherComponentsResults[typeof(Base.BaseObjectsManagementStrategy)]) return false;
                            Entities.Objects.Strategies.Solidness.SolidnessObjectStrategy solidnessStrategy = ((Entities.Objects.Strategies.Solidness.SolidnessObjectStrategy)strategy);
                            SolidnessStatus solidness = solidnessStrategy.Solidness;
                            return solidness.Traverses() || solidnessStrategy.TraversesOtherSolids || IsAdjacencyFree(status.X, status.Y, strategy.StrategyHolder.Object.Width, strategy.StrategyHolder.Object.Height, direction);
                        }

                        /// <summary>
                        ///   <para>
                        ///     Checking the ability to clear movement is directly obtained from the
                        ///       result provided by the same method in <see cref="Base.BaseObjectsManagementStrategy"/>.
                        ///   </para>
                        /// </summary>
                        public override bool CanClearMovement(Dictionary<Type, bool> otherComponentsResults, Entities.Objects.Strategies.ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status)
                        {
                            // Just follows what the BaseStrategy tells
                            return otherComponentsResults[typeof(Base.BaseObjectsManagementStrategy)];
                        }

                        /// <summary>
                        ///   <para>
                        ///     Allocating a movement involves setting the solidness in the tiles "in front" of our object
                        ///       (in terms of movement) according to the solidness of the current object's strategy. 
                        ///   </para>
                        ///   <para>
                        ///     See <see cref="ObjectsManagementStrategy.DoAllocateMovement(Objects.Strategies.ObjectStrategy, ObjectsManagementStrategyHolder.Status, Direction, bool, string)"/>
                        ///       for more information on this method signature and intention.
                        ///   </para>
                        /// </summary>
                        public override void DoAllocateMovement(Entities.Objects.Strategies.ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status, Direction direction, bool continuated, string stage)
                        {
                            switch (stage)
                            {
                                case "AfterMovementAllocation":
                                    Entities.Objects.Strategies.Solidness.SolidnessObjectStrategy solidnessStrategy = ((Entities.Objects.Strategies.Solidness.SolidnessObjectStrategy)strategy);
                                    SolidnessStatus solidness = solidnessStrategy.Solidness;
                                    if (stepType == StepType.Safe)
                                    {
                                        IncrementAdjacent(solidnessStrategy, status, solidness);
                                    }
                                    else if (stepType == StepType.Optimistic)
                                    {
                                        DecrementBackSide(solidnessStrategy, status, solidness);
                                        IncrementAdjacent(solidnessStrategy, status, solidness);
                                    }
                                    break;
                            }
                        }

                        /// <summary>
                        ///   <para>
                        ///     Cancelling the movement involves releasing the solidness of the allocated movement, which involve the tiles
                        ///       "in front" (in terms of movement) of the object.
                        ///   </para>
                        ///   <para>
                        ///     See <see cref="ObjectsManagementStrategy.DoClearMovement(Objects.Strategies.ObjectStrategy, ObjectsManagementStrategyHolder.Status, Direction?, string)"/>
                        ///       for more information on this method signature and intention.
                        ///   </para>
                        /// </summary>
                        public override void DoClearMovement(Entities.Objects.Strategies.ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status, Direction? formerMovement, string stage)
                        {
                            switch (stage)
                            {
                                case "Before":
                                    Entities.Objects.Strategies.Solidness.SolidnessObjectStrategy solidnessStrategy = ((Entities.Objects.Strategies.Solidness.SolidnessObjectStrategy)strategy);
                                    SolidnessStatus solidness = solidnessStrategy.Solidness;
                                    if (stepType == StepType.Safe)
                                    {
                                        DecrementAdjacent(solidnessStrategy, status, solidness);
                                    }
                                    else if (stepType == StepType.Optimistic)
                                    {
                                        IncrementBackSide(solidnessStrategy, status, solidness);
                                        DecrementAdjacent(solidnessStrategy, status, solidness);
                                    }
                                    break;
                            }
                        }

                        /// <summary>
                        ///   <para>
                        ///     When the movement is confirmed, the formed row/column being occupied is released. This is to reflect
                        ///       the fact that those tiles are not being occupied anymore by this object.
                        ///   </para>
                        ///   <para>
                        ///     See <see cref="ObjectsManagementStrategy.DoConfirmMovement(Objects.Strategies.ObjectStrategy, ObjectsManagementStrategyHolder.Status, Direction?, string)"/>
                        ///       for more information on this method signature and intention.
                        ///   </para>
                        /// </summary>
                        public override void DoConfirmMovement(Entities.Objects.Strategies.ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status, Direction? formerMovement, string stage)
                        {
                            switch (stage)
                            {
                                case "AfterPositionChange":
                                    Entities.Objects.Strategies.Solidness.SolidnessObjectStrategy solidnessStrategy = ((Entities.Objects.Strategies.Solidness.SolidnessObjectStrategy)strategy);
                                    SolidnessStatus solidness = solidnessStrategy.Solidness;
                                    if (stepType == StepType.Safe)
                                    {
                                        DecrementOppositeAdjacent(solidnessStrategy, status, solidness);
                                    }
                                    else
                                    {
                                        // Nothing to do here
                                    }
                                    break;
                            }
                        }

                        /// <summary>
                        ///   <para>
                        ///     Refreshes the solidness of the object according to changes in the
                        ///       "solidness" property. It clears the solidness from the previous
                        ///       value, and sets the solidness of the new value.
                        ///   </para>
                        ///   <para>
                        ///     See <see cref="ObjectsManagementStrategy.DoProcessPropertyUpdate(Objects.Strategies.ObjectStrategy, ObjectsManagementStrategyHolder.Status, string, object, object)"/>
                        ///       for more information on this method signature and intention.
                        ///   </para>
                        /// </summary>
                        public override void DoProcessPropertyUpdate(Entities.Objects.Strategies.ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status, string property, object oldValue, object newValue)
                        {
                            Entities.Objects.Strategies.Solidness.SolidnessObjectStrategy solidnessStrategy = ((Entities.Objects.Strategies.Solidness.SolidnessObjectStrategy)strategy);
                            if (property == "solidness" || property == "traversesOtherSolids")
                            {
                                StrategyHolder.MovementCancel(strategy.StrategyHolder);
                                DecrementBody(solidnessStrategy, status, (SolidnessStatus)oldValue);
                                IncrementBody(solidnessStrategy, status, (SolidnessStatus)newValue);
                            }
                        }

                        /// <summary>
                        ///   <para>
                        ///     Processing the teleport of an object involves clearing the solidness in the tile(s) being left, and
                        ///       setting the solidness in the tile(s) being occupied.
                        ///   </para>
                        ///   <para>
                        ///     See <see cref="ObjectsManagementStrategy.DoTeleport(Objects.Strategies.ObjectStrategy, ObjectsManagementStrategyHolder.Status, uint, uint, string)"/>
                        ///       for more information on this method signature and intention.
                        ///   </para>
                        /// </summary>
                        public override void DoTeleport(Entities.Objects.Strategies.ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status, uint x, uint y, string stage)
                        {
                            Entities.Objects.Strategies.Solidness.SolidnessObjectStrategy solidnessStrategy = ((Entities.Objects.Strategies.Solidness.SolidnessObjectStrategy)strategy);
                            SolidnessStatus solidness = solidnessStrategy.Solidness;
                            switch (stage)
                            {
                                case "Before":
                                    DecrementBody(solidnessStrategy, status, solidness);
                                    break;
                                case "AfterPositionChange":
                                    IncrementBody(solidnessStrategy, status, solidness);
                                    break;
                            }
                        }

                        /**
                         * 
                         * Private methods of this particular strategy according to a particular object
                         *   strategy, solidness, and status.
                         * 
                         */

                        private void IncrementBody(Entities.Objects.Strategies.Solidness.SolidnessObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status, SolidnessStatus solidness)
                        {
                            if (solidness.Occupies())
                            {
                                IncrementBody(status.X, status.Y, strategy.StrategyHolder.Object.Width, strategy.StrategyHolder.Object.Height);
                            }
                            else if (solidness.Carves())
                            {
                                DecrementBody(status.X, status.Y, strategy.StrategyHolder.Object.Width, strategy.StrategyHolder.Object.Height);
                            }
                        }

                        private void DecrementBody(Entities.Objects.Strategies.Solidness.SolidnessObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status, SolidnessStatus solidness)
                        {
                            if (solidness.Occupies())
                            {
                                DecrementBody(status.X, status.Y, strategy.StrategyHolder.Object.Width, strategy.StrategyHolder.Object.Height);
                            }
                            else if (solidness.Carves())
                            {
                                IncrementBody(status.X, status.Y, strategy.StrategyHolder.Object.Width, strategy.StrategyHolder.Object.Height);
                            }
                        }

                        private void IncrementAdjacent(Entities.Objects.Strategies.Solidness.SolidnessObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status, SolidnessStatus solidness)
                        {
                            if (solidness.Occupies())
                            {
                                IncrementAdjacent(status.X, status.Y, strategy.StrategyHolder.Object.Width, strategy.StrategyHolder.Object.Height, status.Movement);
                            }
                            else if (solidness.Carves())
                            {
                                DecrementAdjacent(status.X, status.Y, strategy.StrategyHolder.Object.Width, strategy.StrategyHolder.Object.Height, status.Movement);
                            }
                        }

                        private void DecrementAdjacent(Entities.Objects.Strategies.Solidness.SolidnessObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status, SolidnessStatus solidness)
                        {
                            if (solidness.Occupies())
                            {
                                DecrementAdjacent(status.X, status.Y, strategy.StrategyHolder.Object.Width, strategy.StrategyHolder.Object.Height, status.Movement);
                            }
                            else if (solidness.Carves())
                            {
                                IncrementAdjacent(status.X, status.Y, strategy.StrategyHolder.Object.Width, strategy.StrategyHolder.Object.Height, status.Movement);
                            }
                        }

                        private void IncrementBackSide(Entities.Objects.Strategies.Solidness.SolidnessObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status, SolidnessStatus solidness)
                        {
                            if (solidness.Occupies())
                            {
                                IncrementBackSide(status.X, status.Y, strategy.StrategyHolder.Object.Width, strategy.StrategyHolder.Object.Height, status.Movement);
                            }
                            else if (solidness.Carves())
                            {
                                DecrementBackSide(status.X, status.Y, strategy.StrategyHolder.Object.Width, strategy.StrategyHolder.Object.Height, status.Movement);
                            }
                        }

                        private void DecrementBackSide(Entities.Objects.Strategies.Solidness.SolidnessObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status, SolidnessStatus solidness)
                        {
                            if (solidness.Occupies())
                            {
                                DecrementBackSide(status.X, status.Y, strategy.StrategyHolder.Object.Width, strategy.StrategyHolder.Object.Height, status.Movement);
                            }
                            else if (solidness.Carves())
                            {
                                IncrementBackSide(status.X, status.Y, strategy.StrategyHolder.Object.Width, strategy.StrategyHolder.Object.Height, status.Movement);
                            }
                        }

                        private void DecrementOppositeAdjacent(Entities.Objects.Strategies.Solidness.SolidnessObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status, SolidnessStatus solidness)
                        {
                            if (solidness.Occupies())
                            {
                                DecrementAdjacent(status.X, status.Y, strategy.StrategyHolder.Object.Width, strategy.StrategyHolder.Object.Height, status.Movement.Opposite());
                            }
                            else if (solidness.Carves())
                            {
                                IncrementAdjacent(status.X, status.Y, strategy.StrategyHolder.Object.Width, strategy.StrategyHolder.Object.Height, status.Movement.Opposite());
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

                        private void IncrementBackSide(uint x, uint y, uint width, uint height, Direction? direction)
                        {
                            if (!IsHittingEdge(x, y, width, height, direction))
                            {
                                switch (direction)
                                {
                                    case Direction.RIGHT:
                                        solidMask.IncColumn(x, y, height);
                                        break;
                                    case Direction.UP:
                                        solidMask.IncRow(x, y, width);
                                        break;
                                    case Direction.LEFT:
                                        solidMask.IncColumn(x + width - 1, y, height);
                                        break;
                                    case Direction.DOWN:
                                        solidMask.IncRow(x, y + height - 1, width);
                                        break;
                                }
                            }
                        }

                        private void DecrementBackSide(uint x, uint y, uint width, uint height, Direction? direction)
                        {
                            if (!IsHittingEdge(x, y, width, height, direction))
                            {
                                switch (direction)
                                {
                                    case Direction.RIGHT:
                                        solidMask.DecColumn(x, y, height);
                                        break;
                                    case Direction.UP:
                                        solidMask.DecRow(x, y, width);
                                        break;
                                    case Direction.LEFT:
                                        solidMask.DecColumn(x + width - 1, y, height);
                                        break;
                                    case Direction.DOWN:
                                        solidMask.DecRow(x, y + height - 1, width);
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
