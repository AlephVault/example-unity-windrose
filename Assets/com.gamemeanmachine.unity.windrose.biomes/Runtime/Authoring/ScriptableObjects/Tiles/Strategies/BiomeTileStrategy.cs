using AlephVault.Unity.Support.Utils;
using GameMeanMachine.Unity.WindRose.Authoring.ScriptableObjects.Tiles.Strategies;
#if UNITY_EDITOR
using System;
using UnityEditor;
#endif
using UnityEngine;


namespace GameMeanMachine.Unity.WindRose.Biomes
{
    namespace Authoring
    {
        namespace ScriptableObjects
        {
            namespace Tiles
            {
                namespace Strategies
                {
                    /// <summary>
                    ///   Represents the biome this tile belongs to. It
                    ///   may have more than one biome.
                    /// </summary>
                    [CreateAssetMenu(fileName = "NewBiomeTileStrategy", menuName = "Wind Rose/Tile Strategies/Biome",
                        order = 202)]
                    public class BiomeTileStrategy : TileStrategy
                    {
#if UNITY_EDITOR
                        [CustomEditor(typeof(BiomeTileStrategy))]
                        public class BiomeTileStrategyEditor : Editor
                        {
                            // The biome tile strategy being edited.
                            private BiomeTileStrategy component;

                            // The biome indices.
                            private int[] indices;
                            
                            // The biome captions.
                            private string[] captions;

                            private void OnEnable()
                            {
                                component = (BiomeTileStrategy)target;
                                RefreshOptions();
                            }

                            private void RefreshOptions()
                            {
                                if (component.biomeSet != null)
                                {
                                    int index = 0;
                                    indices = new int[component.biomeSet.Count];
                                    captions = new string[component.biomeSet.Count];
                                    foreach (Tuple<string, string> entry in component.biomeSet)
                                    {
                                        indices[index] = index;
                                        captions[index] = $"{entry.Item2} ({entry.Item1})";
                                        index++;
                                    }
                                }
                                else
                                {
                                    indices = null;
                                    captions = null;
                                }
                            }

                            public override void OnInspectorGUI()
                            {
                                if (!component) return;
                                
                                component.biomeSet = (BiomeSet) EditorGUILayout.ObjectField(
                                    "Biome Set", component.biomeSet,
                                    typeof(BiomeSet), component
                                );
                                RefreshOptions();
                                if (component.biomeSet)
                                {
                                    component.biome = (byte)Values.Clamp(
                                        0, EditorGUILayout.IntPopup(
                                            "Biome", component.biome, captions, indices
                                        ),
                                        component.biomeSet.Count - 1
                                    );
                                    component.biomeMask = (ulong)(1 << component.biome);
                                }
                                else
                                {
                                    EditorGUILayout.HelpBox(
                                        "Please select a biome set. Otherwise, this strategy won't work",
                                        MessageType.Warning
                                    );
                                    component.biomeMask = 0;
                                }

                                component.overrideMode = (BiomeOverrideMode)EditorGUILayout.EnumPopup(
                                    "Override Mode", component.overrideMode
                                );
                                
                                EditorUtility.SetDirty(component);
                            }
                        }
#endif

                        /// <summary>
                        ///   The biome set this tile relates to.
                        /// </summary>
                        [SerializeField]
                        private BiomeSet biomeSet;
                        
                        /// <summary>
                        ///   Represents the biome override mode when a tile
                        ///   using a biome appears in a given position:
                        ///   Either extend the current biome (starting at
                        ///   0, or completely replace it).
                        /// </summary>
                        [Serializable]
                        public enum BiomeOverrideMode { Extend, Replace }

                        /// <summary>
                        ///   The per-strategy override mode. Typically,
                        ///   "central" tiles of a biome should have the
                        ///   <see cref="BiomeOverrideMode.Replace"/> mode,
                        ///   while non-central tiles (i.e. remaining 14)
                        ///   would have <see cref="BiomeOverrideMode.Replace"/>.
                        /// </summary>
                        [SerializeField]
                        private BiomeOverrideMode overrideMode = BiomeOverrideMode.Extend;

                        /// <summary>
                        ///   See <see cref="overrideMode"/>.
                        /// </summary>
                        public BiomeOverrideMode OverrideMode => overrideMode;

                        // The cached (1 << biome) value.
                        private ulong biomeMask;

                        /// <summary>
                        ///   The corresponding bitmask for the biome index.
                        /// </summary>
                        public ulong BiomeMask
                        {
                            get
                            {
                                if (biomeSet == null) return 0;

                                if (biomeMask == 0)
                                {
                                    biomeMask = (ulong)(1 << biome);
                                }

                                return biomeMask;
                            }
                        }
                        
                        /// <summary>
                        ///   The biomes this tile contains, with respect
                        ///   to the related <see cref="biomeSet"/>.
                        /// </summary>
                        [SerializeField]
                        private byte biome;

                        /// <summary>
                        ///   See <see cref="biome"/>.
                        /// </summary>
                        public byte Biome => biome;
                    }
                }
            }
        }
    }
}