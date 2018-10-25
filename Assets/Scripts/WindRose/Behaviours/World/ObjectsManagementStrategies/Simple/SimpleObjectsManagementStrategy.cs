using System;
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
            namespace ObjectsManagementStrategies
            {
                namespace Simple
                {
                    /**
                     * SimpleStrategy combines Layout and Solidness strategies
                     *   in one.
                     */
                    [RequireComponent(typeof(Base.LayoutObjectsManagementStrategy))]
                    [RequireComponent(typeof(Solidness.SolidnessObjectsManagementStrategy))]
                    class SimpleObjectsManagementStrategy : ObjectsManagementStrategy
                    {
                        public override bool CanAllocateMovement(Dictionary<Type, bool> otherComponentsResults, ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status, Direction direction, bool continuated)
                        {
                            bool layoutAllowsAllocation = otherComponentsResults[typeof(Base.LayoutObjectsManagementStrategy)];
                            bool solidnessAllowsAllocaction = otherComponentsResults[typeof(Solidness.SolidnessObjectsManagementStrategy)];
                            return layoutAllowsAllocation && solidnessAllowsAllocaction;
                        }

                        public override bool CanClearMovement(Dictionary<Type, bool> otherComponentsResults, ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status)
                        {
                            bool layoutAllowsClearing = otherComponentsResults[typeof(Base.LayoutObjectsManagementStrategy)];
                            bool solidnessAllowsClearing = otherComponentsResults[typeof(Solidness.SolidnessObjectsManagementStrategy)];
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
}
