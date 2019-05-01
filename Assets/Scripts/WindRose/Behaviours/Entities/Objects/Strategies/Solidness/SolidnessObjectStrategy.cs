using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WindRose
{
    namespace Behaviours
    {
        namespace Entities.Objects
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
                    [RequireComponent(typeof(Base.BaseObjectStrategy))]
                    public class SolidnessObjectStrategy : ObjectStrategy
                    {
                        /// <summary>
                        ///   The object's solidness status.
                        /// </summary>
                        [SerializeField]
                        private SolidnessStatus solidness = SolidnessStatus.Solid;

                        /// <summary>
                        ///   Tells whether the object, being solid, can traverse
                        ///     other solids. This property is only meaningful when
                        ///     the current <see cref="solidness"/> is
                        ///     <see cref="SolidnessStatus.Solid"/>.
                        /// </summary>
                        [SerializeField]
                        private bool traversesOtherSolids = false;

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
                                if (oldValue != solidness) PropertyWasUpdated("solidness", oldValue, solidness);
                            }
                        }

                        /// <summary>
                        ///   The object's traversal flag for solid/mask state. See <see cref="traversesOtherSolids"/>.
                        ///   It will notify the counterpart strategy upon value change.
                        /// </summary>
                        public bool TraversesOtherSolids
                        {
                            get { return traversesOtherSolids; }
                            set
                            {
                                var oldValue = traversesOtherSolids;
                                traversesOtherSolids = value;
                                if (oldValue != traversesOtherSolids) PropertyWasUpdated("traversesOtherSolids", oldValue, traversesOtherSolids);
                            }
                        }
                    }

#if UNITY_EDITOR
                    [CustomEditor(typeof(SolidnessObjectStrategy))]
                    [CanEditMultipleObjects]
                    class SolidnessObjectStrategyEditor : Editor
                    {
                        SerializedProperty solidness;
                        SerializedProperty traversesOtherSolids;

                        private void OnEnable()
                        {
                            solidness = serializedObject.FindProperty("solidness");
                            traversesOtherSolids = serializedObject.FindProperty("traversesOtherSolids");
                        }

                        public override void OnInspectorGUI()
                        {
                            serializedObject.Update();

                            EditorGUILayout.PropertyField(solidness);
                            SolidnessStatus solidnessValue = (SolidnessStatus)Enum.GetValues(typeof(SolidnessStatus)).GetValue(solidness.enumValueIndex);
                            if (solidnessValue == SolidnessStatus.Solid || solidnessValue == SolidnessStatus.Mask) EditorGUILayout.PropertyField(traversesOtherSolids);

                            serializedObject.ApplyModifiedProperties();
                        }
                    }
#endif
                }
            }
        }
    }
}
