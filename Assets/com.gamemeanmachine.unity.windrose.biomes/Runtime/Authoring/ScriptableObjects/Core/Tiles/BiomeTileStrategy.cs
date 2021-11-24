using GameMeanMachine.Unity.WindRose.Authoring.ScriptableObjects.Tiles.Strategies;
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
                [CreateAssetMenu(fileName = "NewBiomeTileStrategy", menuName = "Wind Rose/Tile Strategies/Layout", order = 202)]
                public class BiomeTileStrategy : TileStrategy
                {
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