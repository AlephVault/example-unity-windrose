using System;
using UnityEngine;

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
                     * Just a marker to be the counterpart of LayoutStrategy (in map).
                     * See the documentation of that class for more details.
                     */
                    [RequireComponent(typeof(BaseObjectStrategy))]
                    class LayoutObjectStrategy : ObjectStrategy
                    {
                        protected override Type GetCounterpartType()
                        {
                            return typeof(Behaviours.Strategies.Base.LayoutStrategy);
                        }
                    }
                }
            }
        }
    }
}
