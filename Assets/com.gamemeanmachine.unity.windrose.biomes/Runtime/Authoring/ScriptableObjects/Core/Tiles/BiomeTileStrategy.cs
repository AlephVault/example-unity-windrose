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
            namespace Core
            {
                /// <summary>
                ///   Represents the biome this tile belongs to. It
                ///   may have more than one biome.
                /// </summary>
                [CreateAssetMenu(fileName = "NewBiomeTileStrategy", menuName = "Wind Rose/Tile Strategies/Biome", order = 202)]
                public class BiomeTileStrategy : TileStrategy
                {
                    #if UNITY_EDITOR
                    [CustomEditor(typeof(BiomeTileStrategy))]
                    public class BiomeTileStrategyEditor : Editor
                    {
                        // The biome tile strategy being edited
                        private BiomeTileStrategy component;

                        private void OnEnable()
                        {
                            component = (BiomeTileStrategy)target;
                        }

                        public override void OnInspectorGUI()
                        {
                            if (!component) return;
                            component.biomeSet = (BiomeSet)EditorGUILayout.ObjectField(
                                "Biome Set", component.biomeSet,
                                typeof(BiomeSet), component
                            );
                            if (component.biomeSet)
                            {
                                int index = 0;
                                foreach (Tuple<string, string> entry in component.biomeSet)
                                {
                                    bool set = EditorGUILayout.ToggleLeft(
                                        $"{entry.Item2} ({entry.Item1})", 
                                        (component.biome & (1 << index)) != 0
                                    );
                                    if (set)
                                    {
                                        component.biome |= (ushort)(1 << index);
                                    }
                                    else
                                    {
                                        component.biome &= (ushort)~(1 << index);
                                    }

                                    index++;
                                }
                            }
                            else
                            {
                                EditorGUILayout.HelpBox(
                                    "Please select a biome set. Otherwise, this strategy won't work",
                                    MessageType.Warning
                                );
                            }
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
                    ///   The biomes this tile contains, with respect
                    ///   to the related <see cref="biomeSet"/>.
                    /// </summary>
                    [SerializeField]
                    private ushort biome;

                    /// <summary>
                    ///   See <see cref="biome"/>.
                    /// </summary>
                    public ushort Biome => biome;
                }
            }
        }
    }
}