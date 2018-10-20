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
                namespace Solidness
                {
                    using Behaviours.Strategies.Solidness;

                    public class SolidnessObjectStrategy : ObjectStrategy
                    {
                        [SerializeField]
                        private SolidnessStatus solidness = SolidnessStatus.Solid;

                        public override void Initialize()
                        {
                            TriggerPlatform triggerPlatform = StrategyHolder.GetComponent<TriggerPlatform>();
                            if (triggerPlatform != null && solidness != SolidnessStatus.Ghost && solidness != SolidnessStatus.Hole)
                            {
                                solidness = SolidnessStatus.Ghost;
                            }
                        }

                        protected override Type GetCounterpartType()
                        {
                            return typeof(SolidnessStrategy);
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
