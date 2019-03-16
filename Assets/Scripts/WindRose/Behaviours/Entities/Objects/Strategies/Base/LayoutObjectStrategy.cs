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
                namespace Base
                {
                    /// <summary>
                    ///   This strategy is just the counterpart of <see cref="World.ObjectsManagementStrategies.Base.LayoutObjectsManagementStrategy"/>.
                    /// </summary>
                    [RequireComponent(typeof(BaseObjectStrategy))]
                    class LayoutObjectStrategy : ObjectStrategy
                    {
                        /// <summary>
                        ///   The counterpart type is <see cref="World.ObjectsManagementStrategies.Base.LayoutObjectsManagementStrategy"/>.
                        /// </summary>
                        protected override Type GetCounterpartType()
                        {
                            return typeof(World.ObjectsManagementStrategies.Base.LayoutObjectsManagementStrategy);
                        }
                    }
                }
            }
        }
    }
}
