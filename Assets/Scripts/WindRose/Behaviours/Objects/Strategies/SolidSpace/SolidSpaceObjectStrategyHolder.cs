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
                namespace SolidSpace
                {
                    using Behaviours.Strategies.SolidSpace;
                    /**
                     * This object strategy holder will be related to a Solid Space Object Strategy
                     */
                    public class SolidSpaceObjectStrategyHolder : ObjectStrategyHolder
                    {
                        [SerializeField]
                        private SolidnessStatus solidness;

                        protected override ObjectStrategy BuildStrategy()
                        {
                            return new SolidSpaceObjectStrategy(this, solidness);
                        }
                    }
                }
            }
        }
    }
}
