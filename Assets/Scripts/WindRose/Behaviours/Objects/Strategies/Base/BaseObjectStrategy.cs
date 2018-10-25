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
                    /**
                     * Just a marker to be the counterpart of BaseStrategy (in map).
                     * See the documentation of that class for more details.
                     */
                    class BaseObjectStrategy : ObjectStrategy
                    {
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
