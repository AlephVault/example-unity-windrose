using System;
using System.Collections.Generic;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Strategies
        {
            /**
             * This is a map strategy. It will know:
             * - The strategy-holder it is tied to (directly or indirectly).
             * And it will have as behaviour:
             * - List of all methods invoked by the strategy holder that are
             *   related to the current logic.
             */
            public abstract class Strategy
            {
                /**
                 * Each strategy knows its holder.
                 */
                public StrategyHolder StrategyHolder { get; private set; }

                public Strategy(StrategyHolder StrategyHolder)
                {
                    this.StrategyHolder = StrategyHolder;
                }

                /**
                 * Initializing the cell data will require initializing masks and other arrays, in a per-strategy
                 *   basis.
                 */
                public abstract void InitGlobalCellData();

                /**
                 * This method tells whether the strategy has cells to compute, or not.
                 */
                protected virtual bool ComputesCellsData()
                {
                    return true;
                }

                /**
                 * This method updates all the cells in the strategy. It depends on the method to update a
                 *   single cell. This is an utility method that will frequently be used when
                 */
                private void ComputeCellsData()
                {
                    for (uint y = 0; y < StrategyHolder.Map.Height; y++)
                    {
                        for (uint x = 0; x < StrategyHolder.Map.Width; x++)
                        {
                            ComputeCellData(x, y);
                        }
                    }
                }

                /**
                 * This method initializes the current strategy.
                 * [For computed strategies, it will also call initialize on children]
                 */
                public virtual void Initialize()
                {
                    InitGlobalCellData();
                    if (ComputesCellsData())
                    {
                        ComputeCellsData();
                    }
                }

                /**
                 * Updating a strategy actually computes a single cell. It is intended to be run after a tilemap
                 *   was changed a single tile, or in per-cell basis on initialization.
                 */
                public abstract void ComputeCellData(uint x, uint y);

                /*************************************************************************************************
                 * 
                 * Attaching an object (strategy).
                 * 
                 *************************************************************************************************/

                /**
                 * Object strategy allowance. It will tell whether it accepts, or not, a certain strategy to be passed.
                 */
                public abstract bool AcceptsObjectStrategy(Objects.Strategies.ObjectStrategy strategy);

                /**
                 * Object strategy was added. Compute relevant data here.
                 */
                public abstract void AttachedStratergy(Objects.Strategies.ObjectStrategy strategy, StrategyHolder.Status status);

                /**
                 * Object strategy is to be removed. Compute relevant data here.
                 */
                public abstract void DetachedStratergy(Objects.Strategies.ObjectStrategy strategy, StrategyHolder.Status status);

                /*************************************************************************************************
                 * 
                 * Starting the movement of an object (strategy) in a direction and telling whether it is the
                 *   continuation of a former movement (this will happen in Movable component).
                 * 
                 *************************************************************************************************/

                /**
                 * Executes the actual movement allocation.
                 */
                public bool AllocateMovement(Objects.Strategies.ObjectStrategy strategy, StrategyHolder.Status status, Types.Direction direction, bool continuated = false)
                {
                    if (CanAllocateMovement(strategy, status, direction, continuated))
                    {
                        DoAllocateMovement(strategy, status, direction, continuated, "Before");
                        status.Movement = direction;
                        DoAllocateMovement(strategy, status, direction, continuated, "AfterMovementAllocation");
                        strategy.TriggerEvent("OnMovementStarted", direction);
                        DoAllocateMovement(strategy, status, direction, continuated, "After");
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                /**
                 * Tells whether movement can be allocated.
                 */
                public abstract bool CanAllocateMovement(Objects.Strategies.ObjectStrategy strategy, StrategyHolder.Status status, Types.Direction direction, bool continuated);

                /**
                 * Performs a dumb bounds-check on the object's position, size, and current map.
                 */
                protected bool IsHittingEdge(Objects.Positionable positionable, StrategyHolder.Status status, Types.Direction direction)
                {
                    switch (direction)
                    {
                        case Types.Direction.LEFT:
                            return status.X == 0;
                        case Types.Direction.UP:
                            return status.Y + positionable.Height == StrategyHolder.Map.Height;
                        case Types.Direction.RIGHT:
                            return status.X + positionable.Width == StrategyHolder.Map.Width;
                        case Types.Direction.DOWN:
                            return status.Y == 0;
                    }
                    return false;
                }

                /**
                 * You have to define this method. Ask conditionally for "Before", "AfterMovementAllocation", "After".
                 */
                public abstract void DoAllocateMovement(Objects.Strategies.ObjectStrategy strategy, StrategyHolder.Status status, Types.Direction direction, bool continuated, string stage);

                /*************************************************************************************************
                 * 
                 * Cancelling the movement of an object (strategy).
                 * 
                 *************************************************************************************************/

                /**
                 * Executes the actual movement clearing.
                 */
                public bool ClearMovement(Objects.Strategies.ObjectStrategy strategy, StrategyHolder.Status status)
                {
                    if (CanClearMovement(strategy, status))
                    {
                        Types.Direction? formerMovement = status.Movement;
                        DoClearMovement(strategy, status, formerMovement, "Before");
                        status.Movement = null;
                        DoClearMovement(strategy, status, formerMovement, "AfterMovementClear");
                        strategy.TriggerEvent("OnMovementCancelled", formerMovement);
                        DoClearMovement(strategy, status, formerMovement, "Before");
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                /**
                 * Override it to use a different criteria for allowing movement cancel.
                 */
                public virtual bool CanClearMovement(Objects.Strategies.ObjectStrategy strategy, StrategyHolder.Status status)
                {
                    return status.Movement != null;
                }

                /**
                 * You have to define this method. Ask conditionally for "Before", "AfterMovementClear", "After".
                 */
                public abstract void DoClearMovement(Objects.Strategies.ObjectStrategy strategy, StrategyHolder.Status status, Types.Direction? formerMovement, string stage);

                /*************************************************************************************************
                 * 
                 * Finishing the movement of an object (strategy), if any.
                 * 
                 *************************************************************************************************/

                /**
                 * You have to define this method. Ask conditionally for "Before", "AfterPositionChange", "AfterMovementClear", "After".
                 */
                public abstract void DoConfirmMovement(Objects.Strategies.ObjectStrategy strategy, StrategyHolder.Status status, Types.Direction? formerMovement, string stage);

                /*************************************************************************************************
                 * 
                 * Teleports the object strategy to another position in the map.
                 * 
                 *************************************************************************************************/

                /**
                 * You have to define this method. Ask conditionally for stages "Before", "AfterPositionChange", "After".
                 */
                public abstract void DoTeleport(Objects.Strategies.ObjectStrategy strategy, StrategyHolder.Status status, uint x, uint y, string stage);

                /*************************************************************************************************
                 * 
                 * Updates according to particular data change. These fields exist in the strategy.
                 * 
                 *************************************************************************************************/

                /**
                 * You have to define this method.
                 */
                public abstract void DoProcessPropertyUpdate(Objects.Strategies.ObjectStrategy strategy, StrategyHolder.Status status, string property, object oldValue, object newValue);
            }
        }
    }
}
