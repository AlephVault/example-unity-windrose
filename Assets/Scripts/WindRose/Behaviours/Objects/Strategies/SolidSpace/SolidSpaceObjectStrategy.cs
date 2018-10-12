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

                    public class SolidSpaceObjectStrategy : ObjectStrategy
                    {
                        [SerializeField]
                        private SolidnessStatus solidness = SolidnessStatus.Solid;

                        public SolidSpaceObjectStrategy(ObjectStrategyHolder StrategyHolder, SolidnessStatus solidness) : base(StrategyHolder) {
                            this.solidness = solidness;
                        }

                        public override void Initialize()
                        {
                            TriggerPlatform triggerPlatform = StrategyHolder.GetComponent<TriggerPlatform>();
                            if (triggerPlatform != null && solidness != SolidnessStatus.Ghost && solidness != SolidnessStatus.Hole)
                            {
                                solidness = SolidnessStatus.Ghost;
                            }
                        }

                        public SolidnessStatus Solidness
                        {
                            get { return solidness; }
                            set
                            {
                                var oldValue = solidness;
                                PropertyWasUpdated("solidness", oldValue, value);
                            }
                        }
                    }
                }
            }
        }
    }
}
