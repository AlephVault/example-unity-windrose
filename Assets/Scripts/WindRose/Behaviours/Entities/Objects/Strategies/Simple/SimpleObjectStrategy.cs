using System;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Entities.Objects
        {
            namespace Strategies
            {
                namespace Simple
                {
                    /// <summary>
                    ///   Simple object strategy is just a combination of
                    ///   <see cref="Base.LayoutObjectStrategy"/> and
                    ///   <see cref="Solidness.SolidnessObjectStrategy"/>.
                    ///   Its counterpart type is
                    ///   <see cref="World.ObjectsManagementStrategies.Simple.SimpleObjectsManagementStrategy"/>.
                    /// </summary>
                    [RequireComponent(typeof(Base.BaseObjectStrategy))]
                    [RequireComponent(typeof(Solidness.SolidnessObjectStrategy))]
                    class SimpleObjectStrategy : ObjectStrategy
                    {
                        /// <summary>
                        ///   Its counterpart type is
                        ///   <see cref="World.ObjectsManagementStrategies.Simple.SimpleObjectsManagementStrategy"/>.
                        /// </summary>
                        protected override Type GetCounterpartType()
                        {
                            return typeof(World.ObjectsManagementStrategies.Simple.SimpleObjectsManagementStrategy);
                        }
                    }
                }
            }
        }
    }
}
