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
                namespace Simple
                {
                    [RequireComponent(typeof(Base.BaseObjectStrategy))]
                    [RequireComponent(typeof(Solidness.SolidnessObjectStrategy))]
                    class SimpleObjectStrategy : ObjectStrategy
                    {
                        protected override Type GetCounterpartType()
                        {
                            return typeof(Behaviours.Strategies.Simple.SimpleStrategy);
                        }
                    }
                }
            }
        }
    }
}
