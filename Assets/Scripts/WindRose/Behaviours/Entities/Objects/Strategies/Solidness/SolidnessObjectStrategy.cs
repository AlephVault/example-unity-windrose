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
                    using Support.Utils;
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

                        /// <summary>
                        ///   Defines the mask of the object, which is specified as a
                        ///     paragraph of (height) lines, each having (width) characters
                        ///     being S (solid), G (Ghost) or H (Hole), case-insensitive.
                        /// </summary>
                        [SerializeField]
                        [TextArea(5, 5)]
                        private string initialMask = "";

                        // This is the parsed mask, after proper clamping.
                        private SolidnessStatus[,] mask;

                        // Tells whether this object is also a TriggerPlatform.
                        private bool isPlatform;

                        private void ClampSolidness()
                        {
                            if (isPlatform && solidness != SolidnessStatus.Ghost && solidness != SolidnessStatus.Hole)
                            {
                                solidness = SolidnessStatus.Ghost;
                            }
                        }

                        // Clamps the mask from the initial text
                        private void InitMaskFromText()
                        {
                            string[] lines = initialMask.Split('\n');
                            uint linesCount = Values.Min(Object.Height, (uint)lines.Length);

                            mask = new SolidnessStatus[Object.Width, Object.Height];

                            for(uint lineIdx = 0; lineIdx < linesCount; lineIdx++)
                            {
                                string line = lines[lineIdx].ToLower();
                                uint lineSize = Values.Min(Object.Width, (uint)line.Length);
                                for(uint colIdx = 0; colIdx < lineSize; colIdx++)
                                {
                                    char currentChar = line[(int)colIdx];
                                    switch(currentChar)
                                    {
                                        case 's':
                                            mask[colIdx, lineIdx] = SolidnessStatus.Solid;
                                            break;
                                        case 'h':
                                            mask[colIdx, lineIdx] = SolidnessStatus.Hole;
                                            break;
                                        default:
                                            mask[colIdx, lineIdx] = SolidnessStatus.Ghost;
                                            break;
                                    }
                                }
                                for(uint colIdx = lineSize; colIdx < Object.Width; colIdx++)
                                {
                                    mask[colIdx, lineIdx] = SolidnessStatus.Ghost;
                                }
                            }
                            for(uint lineIdx = linesCount; lineIdx < Object.Height; lineIdx++)
                            {
                                for(uint colIdx = 0; colIdx < Object.Width; colIdx++)
                                {
                                    mask[colIdx, lineIdx] = SolidnessStatus.Ghost;
                                }
                            }
                        }

                        // Clamps the mask from the current one
                        private void ClampMask()
                        {
                            if (mask == null)
                            {
                                mask = new SolidnessStatus[Object.Width, Object.Height];
                                for(uint i = 0; i < Object.Width; i++)
                                {
                                    for(uint j = 0; j < Object.Height; j++)
                                    {
                                        mask[i, j] = SolidnessStatus.Ghost;
                                    }
                                }
                                return;
                            }

                            SolidnessStatus[,] newMask = new SolidnessStatus[Object.Width, Object.Height];

                            uint linesCount = Values.Min(Object.Height, (uint)mask.GetLength(1));
                            for (uint lineIdx = 0; lineIdx < linesCount; lineIdx++)
                            {
                                uint lineSize = Values.Min(Object.Width, (uint)mask.GetLength(0));
                                for (uint colIdx = 0; colIdx < lineSize; colIdx++)
                                {
                                    if (mask[colIdx, lineIdx] == SolidnessStatus.Mask)
                                    {
                                        newMask[colIdx, lineIdx] = SolidnessStatus.Ghost;
                                    }
                                    else
                                    {
                                        newMask[colIdx, lineIdx] = mask[colIdx, lineIdx];
                                    }
                                }
                                for (uint colIdx = lineSize; colIdx < Object.Width; colIdx++)
                                {
                                    newMask[colIdx, lineIdx] = SolidnessStatus.Ghost;
                                }
                            }
                            for (uint lineIdx = linesCount; lineIdx < Object.Height; lineIdx++)
                            {
                                for (uint colIdx = 0; colIdx < Object.Width; colIdx++)
                                {
                                    newMask[colIdx, lineIdx] = SolidnessStatus.Ghost;
                                }
                            }

                            mask = newMask;
                        }

                        protected override void Awake()
                        {
                            base.Awake();
                            InitMaskFromText();
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
                        ///   Gets (a clone of) the current mask / changes the current mask.
                        ///   The new mask will be clamped and filled (with "ghost" values)
                        ///     appropriately.
                        /// </summary>
                        public SolidnessStatus[,] Mask
                        {
                            get { return (SolidnessStatus[,])mask.Clone(); }
                            set
                            {
                                if (mask == value) return;
                                var oldValue = mask;
                                mask = value;
                                ClampMask();
                                PropertyWasUpdated("mask", oldValue, mask);
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
                        SerializedProperty mask;

                        private void OnEnable()
                        {
                            solidness = serializedObject.FindProperty("solidness");
                            traversesOtherSolids = serializedObject.FindProperty("traversesOtherSolids");
                            mask = serializedObject.FindProperty("initialMask");
                        }

                        public override void OnInspectorGUI()
                        {
                            serializedObject.Update();

                            EditorGUILayout.PropertyField(solidness);
                            SolidnessStatus solidnessValue = (SolidnessStatus)Enum.GetValues(typeof(SolidnessStatus)).GetValue(solidness.enumValueIndex);
                            if (solidnessValue == SolidnessStatus.Solid) EditorGUILayout.PropertyField(traversesOtherSolids);
                            if (solidnessValue == SolidnessStatus.Mask) EditorGUILayout.PropertyField(mask);

                            serializedObject.ApplyModifiedProperties();
                        }
                    }
#endif
                }
            }
        }
    }
}
