using System;
using System.Collections.Generic;
using UnityEngine;
using WindRose.Behaviours.Objects.Strategies;
using WindRose.Types;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Strategies
        {
            namespace Simple
            {
                /**
                 * SimpleStrategy combines Layout and Solidness strategies
                 *   in one.
                 */
                [RequireComponent(typeof(Base.LayoutStrategy))]
                [RequireComponent(typeof(Solidness.SolidnessStrategy))]
                class SimpleStrategy : Strategy
                {
                    public override bool CanAllocateMovement(Dictionary<Type, bool> otherComponentsResults, ObjectStrategy strategy, StrategyHolder.Status status, Direction direction, bool continuated)
                    {
                        bool layoutAllowsAllocation = otherComponentsResults[typeof(Base.LayoutStrategy)];
                        bool solidnessAllowsAllocaction = otherComponentsResults[typeof(Solidness.SolidnessStrategy)];
                        return layoutAllowsAllocation && solidnessAllowsAllocaction;
                    }

                    public override bool CanClearMovement(Dictionary<Type, bool> otherComponentsResults, ObjectStrategy strategy, StrategyHolder.Status status)
                    {
                        bool layoutAllowsClearing = otherComponentsResults[typeof(Base.LayoutStrategy)];
                        bool solidnessAllowsClearing = otherComponentsResults[typeof(Solidness.SolidnessStrategy)];
                        return layoutAllowsClearing && solidnessAllowsClearing;
                    }

                    protected override Type GetCounterpartType()
                    {
                        return typeof(Objects.Strategies.Simple.SimpleObjectStrategy);
                    }
                }
            }
        }
    }
}
