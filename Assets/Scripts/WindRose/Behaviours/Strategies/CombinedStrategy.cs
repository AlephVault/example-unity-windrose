using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Strategies
        {
            /**
             * This is a combined object strategy. It is intended to initialize
             *   somehow the inner strategies to be part of this super-strategy.
             *   
             * Then it may define fields that actually delegate to inner strategies.
             * An example:
             * - We have a SolidSpaceStrategy which works with the Solidness field.
             * - We have a WaterAwareStrategy which works with water tiles.
             * - We may know how to work with flow tiles - forced movements for ships.
             * 
             * This class is abstract since all the methods act over the children and
             *   do not add their own logic. That is up to the user, who could decide
             *   whether add the logic at the beginning or the end of each call.
             */
            public abstract class CombinedStrategy : Strategy
            {
                /**
                 * This strategy also knows which sub-strategies does it handle.
                 *   ORDER WILL BE IMPORTANT HERE.
                 */
                private Strategy[] childrenStrategies = null;

                /**
                 * This constructor will take the holder for the base constructor
                 *   and also a function to execute to initialize the children
                 *   strategies. This function should be created as a closure from
                 *   the child class (this closure will be made on-demand and based
                 *   on the input data that the strategy could gather beyond the 
                 *   strategy holder, while getting the strategy holder as the same
                 *   parameter that is present in the constructor.
                 */
                public CombinedStrategy(StrategyHolder StrategyHolder, Func<StrategyHolder, Strategy[]> initializer) : base(StrategyHolder)
                {
                    childrenStrategies = initializer(StrategyHolder);
                }

                /**
                 * Each-iterating function over the strategies.
                 */
                private void Each(Action<Strategy> callback)
                {
                    foreach(Strategy strategy in childrenStrategies)
                    {
                        callback(strategy);
                    }
                }

                /**
                 * All-iterating function over the strategies.
                 */
                private bool All(Predicate<Strategy> callback)
                {
                    foreach (Strategy strategy in childrenStrategies)
                    {
                        if (!callback(strategy)) return false;
                    }
                    return true;
                }

                /**
                 * This method initializes the current strategy.
                 * [For computed strategies, it will also call initialize on children]
                 */
                public override void Initialize()
                {
                    Each(delegate (Strategy strategy)
                    {
                        strategy.Initialize();
                    });
                }

                /**
                 * Updating a strategy actually computes a single cell. It is intended to be run after a tilemap
                 *   was changed a single tile, or in per-cell basis on initialization.
                 */
                public override void ComputeCellData(uint x, uint y)
                {
                    Each(delegate (Strategy strategy)
                    {
                        strategy.ComputeCellData(x, y);
                    });
                }

                /*************************************************************************************************
                 * 
                 * Attaching an object (strategy).
                 * 
                 *************************************************************************************************/

                /**
                 * Object strategy allowance. It will tell whether it accepts, or not, a certain strategy to be passed.
                 */
                public override bool AcceptsObjectStrategy(Objects.Strategies.ObjectStrategy objectStrategy)
                {
                    if (!(objectStrategy is Objects.Strategies.CombinedObjectStrategy)) return false;

                    int index = 0;
                    return All(delegate (Strategy strategy)
                    {
                        Objects.Strategies.ObjectStrategy childObjectStrategy = ((Objects.Strategies.CombinedObjectStrategy)objectStrategy)[index++];
                        return strategy.AcceptsObjectStrategy(childObjectStrategy);
                    });
                }

                /**
                 * Object strategy was added. Compute relevant data here.
                 */
                public override void AttachedStratergy(Objects.Strategies.ObjectStrategy objectStrategy, StrategyHolder.Status status)
                {
                    Each(delegate (Strategy strategy)
                    {
                        strategy.AttachedStratergy(objectStrategy, status);
                    });
                }

                /**
                 * Object strategy is to be removed. Compute relevant data here.
                 */
                public override void DetachedStratergy(Objects.Strategies.ObjectStrategy objectStrategy, StrategyHolder.Status status)
                {
                    Each(delegate (Strategy strategy)
                    {
                        strategy.DetachedStratergy(objectStrategy, status);
                    });
                }

                /*************************************************************************************************
                 * 
                 * Starting the movement of an object (strategy) in a direction and telling whether it is the
                 *   continuation of a former movement (this will happen in Movable component).
                 * 
                 *************************************************************************************************/

                /**
                 * Tells whether movement can be allocated.
                 */
                public override bool CanAllocateMovement(Objects.Strategies.ObjectStrategy objectStrategy, StrategyHolder.Status status, Types.Direction direction, bool continuated)
                {
                    return All(delegate (Strategy strategy)
                    {
                        return strategy.CanAllocateMovement(objectStrategy, status, direction, continuated);
                    });
                }

                /**
                 * You have to define this method. Ask conditionally for "Before", "AfterMovementAllocation", "After".
                 */
                public override void DoAllocateMovement(Objects.Strategies.ObjectStrategy objectStrategy, StrategyHolder.Status status, Types.Direction direction, bool continuated, string stage)
                {
                    Each(delegate (Strategy strategy)
                    {
                        strategy.DoAllocateMovement(objectStrategy, status, direction, continuated, stage);
                    });
                }

                /*************************************************************************************************
                 * 
                 * Cancelling the movement of an object (strategy).
                 * 
                 *************************************************************************************************/

                /**
                 * Override it to use a different criteria for allowing movement cancel.
                 */
                public override bool CanClearMovement(Objects.Strategies.ObjectStrategy objectStrategy, StrategyHolder.Status status)
                {
                    return All(delegate (Strategy strategy)
                    {
                        return strategy.CanClearMovement(objectStrategy, status);
                    });
                }

                /**
                 * You have to define this method. Ask conditionally for "Before", "AfterMovementClear", "After".
                 */
                public override void DoClearMovement(Objects.Strategies.ObjectStrategy objectStrategy, StrategyHolder.Status status, Types.Direction? formerMovement, string stage)
                {
                    Each(delegate (Strategy strategy)
                    {
                        strategy.DoClearMovement(objectStrategy, status, formerMovement, stage);
                    });
                }

                /*************************************************************************************************
                 * 
                 * Finishing the movement of an object (strategy), if any.
                 * 
                 *************************************************************************************************/

                /**
                 * You have to define this method. Ask conditionally for "Before", "AfterPositionChange", "AfterMovementClear", "After".
                 */
                public override void DoConfirmMovement(Objects.Strategies.ObjectStrategy objectStrategy, StrategyHolder.Status status, Types.Direction? formerMovement, string stage)
                {
                    Each(delegate (Strategy strategy)
                    {
                        strategy.DoConfirmMovement(objectStrategy, status, formerMovement, stage);
                    });
                }

                /*************************************************************************************************
                 * 
                 * Teleports the object strategy to another position in the map.
                 * 
                 *************************************************************************************************/

                /**
                 * You have to define this method. Ask conditionally for stages "Before", "AfterPositionChange", "After".
                 */
                public override void DoTeleport(Objects.Strategies.ObjectStrategy objectStrategy, StrategyHolder.Status status, uint x, uint y, string stage)
                {
                    Each(delegate (Strategy strategy)
                    {
                        strategy.DoTeleport(objectStrategy, status, x, y, stage);
                    });
                }

                /*************************************************************************************************
                 * 
                 * Updates according to particular data change. These fields exist in the strategy.
                 * 
                 *************************************************************************************************/

                /**
                 * You have to define this method.
                 */
                public override void DoProcessPropertyUpdate(Objects.Strategies.ObjectStrategy objectStrategy, StrategyHolder.Status status, string property, object oldValue, object newValue)
                {
                    Each(delegate (Strategy strategy)
                    {
                        strategy.DoProcessPropertyUpdate(objectStrategy, status, property, oldValue, newValue);
                    });
                }
            }
        }
    }
}
