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
                    using World.ObjectsManagementStrategies.Solidness;

                    /// <summary>
                    ///   Solidness strategy keeps the solidness state of this
                    ///     object. By default, it will be solid. See the
                    ///     <see cref="Solidness"/> property and
                    ///     <see cref="solidness"/> field for more details.
                    /// </summary>
                    public class SolidnessObjectStrategy : ObjectStrategy
                    {
                        /// <summary>
                        ///   The object's solidness status.
                        /// </summary>
                        [SerializeField]
                        private SolidnessStatus solidness = SolidnessStatus.Solid;

                        // Tells whether this object is also a TriggerPlatform.
                        private bool isPlatform;

                        private void ClampSolidness()
                        {
                            if (isPlatform && solidness != SolidnessStatus.Ghost && solidness != SolidnessStatus.Hole)
                            {
                                solidness = SolidnessStatus.Ghost;
                            }
                        }

                        /// <summary>
                        ///   Upon initialization, if this object is also a <see cref="TriggerPlatform"/>
                        ///     then the solidness will be changed to <see cref="SolidnessStatus.Ghost"/> unless
                        ///     it is <see cref="SolidnessStatus.Hole"/>.
                        /// </summary>
                        public override void Initialize()
                        {
                            TriggerPlatform triggerPlatform = StrategyHolder.GetComponent<TriggerPlatform>();
                            isPlatform = triggerPlatform != null;
                            ClampSolidness();
                        }

                        /// <summary>
                        ///   Its counterpart strategy is <see cref="SolidnessObjectsManagementStrategy"/>.
                        /// </summary>
                        /// <returns>The counterpart type</returns>
                        protected override Type GetCounterpartType()
                        {
                            return typeof(SolidnessObjectsManagementStrategy);
                        }

                        /// <summary>
                        ///   <para>
                        ///     The object's solidness status property. It will notify
                        ///       the counterpart strategy upon value change.
                        ///   </para>
                        ///   <para>
                        ///     If the object is a platform, the solidness will only be allowed
                        ///       to be <see cref="SolidnessStatus.Ghost"/> or
                        ///       <see cref="SolidnessStatus.Hole"/>.
                        ///   </para>
                        /// </summary>
                        public SolidnessStatus Solidness
                        {
                            get { return solidness; }
                            set
                            {
                                var oldValue = solidness;
                                solidness = value;
                                ClampSolidness();
                                PropertyWasUpdated("solidness", oldValue, solidness);
                            }
                        }
                    }
                }
            }
        }
    }
}
