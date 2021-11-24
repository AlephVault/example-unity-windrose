using System;
using System.Collections;
using System.Collections.Generic;
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
                ///   Represents a read-only bundle having all of the
                ///   biomes for a given map layout.
                /// </summary>
                [CreateAssetMenu(fileName = "NewBiomeSet", menuName = "Wind Rose/Biome Set", order = 210)]
                public class BiomeSet : ScriptableObject, IEnumerable<Tuple<string, string>>
                {
                    /// <summary>
                    ///   A single biome entry.
                    /// </summary>
                    [Serializable]
                    public class BiomeEntry
                    {
                        /// <summary>
                        ///   The unique code of the biome.
                        /// </summary>
                        [SerializeField]
                        private string code;

                        /// <summary>
                        ///   The visual name of the biome, in english.
                        /// </summary>
                        [SerializeField]
                        private string name;

                        /// <summary>
                        ///   See <see cref="code"/>.
                        /// </summary>
                        public string Code => code;

                        /// <summary>
                        ///   See <see cref="name"/>.
                        /// </summary>
                        public string Name => name;
                    }

                    // A dictionary of index by biome code.
                    private Dictionary<string, int> biomeIndexByCode = new Dictionary<string, int>();

                    // A list of biome by index.
                    [SerializeField]
                    private List<BiomeEntry> biomes = new List<BiomeEntry>();
                    
                    /// <summary>
                    ///   Returns the biome entry for a certain bit index.
                    /// </summary>
                    /// <param name="index">The chosen bit index</param>
                    public BiomeEntry this[int index] => biomes[index];

                    /// <summary>
                    ///   Returns the biome index by the given code.
                    /// </summary>
                    /// <param name="code"></param>
                    public int this[string code] => biomeIndexByCode[code];

                    /// <summary>
                    ///   Iterates over the list of biomes.
                    /// </summary>
                    /// <returns>An enumerator of (code, name) pairs</returns>
                    public IEnumerator<Tuple<string, string>> GetEnumerator()
                    {
                        foreach (BiomeEntry entry in biomes)
                        {
                            yield return new Tuple<string, string>(entry.Code, entry.Name);
                        }
                    }

                    IEnumerator IEnumerable.GetEnumerator()
                    {
                        return GetEnumerator();
                    }

                    /// <summary>
                    ///   Gets the count of biomes.
                    /// </summary>
                    public int Count => biomes.Count;

                    /// <summary>
                    ///   Gets the count of biomes.
                    /// </summary>
                    public int Length => biomes.Count;

                    private void OnEnable()
                    {
                        biomeIndexByCode.Clear();
                        int count = 0;
                        foreach (BiomeEntry entry in biomes)
                        {
                            biomeIndexByCode.Add(entry.Code, count++);
                        }
                    }
                }
            }
        }
    }
}
