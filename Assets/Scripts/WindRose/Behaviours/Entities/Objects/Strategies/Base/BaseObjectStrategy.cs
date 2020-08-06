using System;

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
                    ///   This strategy is just the counterpart of <see cref="World.Layers.Objects.ObjectsManagementStrategies.Base.BaseObjectsManagementStrategy"/>.
                    /// </summary>
                    public class BaseObjectStrategy : ObjectStrategy
                    {
                        /// <summary>
                        ///   The counterpart type is <see cref="World.Layers.Objects.ObjectsManagementStrategies.Base.BaseObjectsManagementStrategy"/>.
                        /// </summary>
                        protected override Type GetCounterpartType()
                        {
                            return typeof(World.Layers.Objects.ObjectsManagementStrategies.Base.BaseObjectsManagementStrategy);
                        }
                    }
                }
            }
        }
    }
}
