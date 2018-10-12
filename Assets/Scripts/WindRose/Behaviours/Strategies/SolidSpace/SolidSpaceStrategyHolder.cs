using System;
using System.Collections.Generic;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Strategies
        {
            namespace SolidSpace
            {
                /**
                 * This map strategy holder is related to the SolidSpaceStrategy.
                 */
                [RequireComponent(typeof(Map))]
                public class SolidSpaceStrategyHolder : StrategyHolder
                {
                    /**
                     * And also each strategy holder knows how to instantiate its strategy.
                     */
                    protected override Strategy BuildStrategy()
                    {
                        return new SolidSpaceStrategy(this);
                    }
                }
            }
        }
    }
}
