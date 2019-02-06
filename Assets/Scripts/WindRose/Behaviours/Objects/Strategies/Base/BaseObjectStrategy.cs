using System;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Objects
        {
            namespace Strategies
            {
                namespace Base
                {
                    /// <summary>
                    ///   This strategy is just the counterpart of <see cref="World.ObjectsManagementStrategies.Base.BaseObjectsManagementStrategy"/>.
                    /// </summary>
                    class BaseObjectStrategy : ObjectStrategy
                    {
                        /// <summary>
                        ///   The counterpart type is <see cref="World.ObjectsManagementStrategies.Base.BaseObjectsManagementStrategy"/>.
                        /// </summary>
                        protected override Type GetCounterpartType()
                        {
                            return typeof(World.ObjectsManagementStrategies.Base.BaseObjectsManagementStrategy);
                        }
                    }
                }
            }
        }
    }
}
