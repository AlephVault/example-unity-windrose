using System;
using System.Collections;
using System.Collections.Generic;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Objects.Strategies;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.World.Layers.Objects;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.World.Layers.Objects.ObjectsManagementStrategies;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.World.Layers.Objects.ObjectsManagementStrategies.Base;
using GameMeanMachine.Unity.WindRose.Authoring.ScriptableObjects.Tiles;
using GameMeanMachine.Unity.WindRose.Biomes.Authoring.Behaviours.Entities.Objects.Strategies;
using GameMeanMachine.Unity.WindRose.Biomes.Authoring.ScriptableObjects.Tiles;
using GameMeanMachine.Unity.WindRose.Biomes.Authoring.ScriptableObjects.Tiles.Strategies;
using GameMeanMachine.Unity.WindRose.Biomes.Types;
using GameMeanMachine.Unity.WindRose.Types;
using UnityEngine;


namespace GameMeanMachine.Unity.WindRose.Biomes
{
    namespace Authoring
    {
        namespace Behaviours
        {
            namespace World
            {
                namespace Layers
                {
                    namespace Objects
                    {
                        namespace ObjectsManagementStrategies
                        {
                            /// <summary>
                            ///   A biome strategy checks many additional "layout" settings
                            ///   that can be switched for a given object (e.g. moving from
                            ///   "walking/running" to "swimming/sailing" involves also a
                            ///   biome change and will be tested respectively against a
                            ///   water layout or walkable layout).
                            /// </summary>
                            [RequireComponent(typeof(LayoutObjectsManagementStrategy))]
                            public class BiomeObjectsManagementStrategy : ObjectsManagementStrategy
                            {
                                /// <summary>
                                ///   The biome set this strategy relates to.
                                /// </summary>
                                [SerializeField]
                                private BiomeSet biomeSet;
                                
                                // This variable is set on strategy initialization.
                                private Bitmask[] biomes;

                                private LayoutObjectsManagementStrategy layoutObjectsManagementStrategy;

                                protected override void Awake()
                                {
                                    base.Awake();
                                    layoutObjectsManagementStrategy = GetComponent<LayoutObjectsManagementStrategy>();
                                }

                                // For a given biome bitmask, checks whether any square in any adjacent side
                                // is unset (i.e. lacks of biome).
                                private bool IsAdjacencyUnset(Bitmask bitmask, uint x, uint y, uint width, uint height, Direction? direction)
                                {
                                    switch (direction)
                                    {
                                        case Direction.LEFT:
                                            return bitmask.GetColumn(x - 1, y, y + height - 1, Bitmask.CheckType.ANY_UNSET);
                                        case Direction.DOWN:
                                            return bitmask.GetRow(x, x + width - 1, y - 1, Bitmask.CheckType.ANY_UNSET);
                                        case Direction.RIGHT:
                                            return bitmask.GetColumn(x + width, y, y + height - 1, Bitmask.CheckType.ANY_UNSET);
                                        case Direction.UP:
                                            return bitmask.GetRow(x, x + width - 1, y + height, Bitmask.CheckType.ANY_UNSET);
                                        default:
                                            return true;
                                    }
                                }

                                protected override Type GetCounterpartType()
                                {
                                    return typeof(BiomeObjectStrategy);
                                }

                                /// <summary>
                                ///   Tests whether the new strategy is not null and also has the same
                                ///   biome set of this management strategy
                                /// </summary>
                                /// <param name="otherComponentsResults">
                                ///   A dictionary holding the calculated value, for this method, in the dependencies. You can -and often
                                ///     WILL- also take those values into account for this calculation</param>
                                /// <param name="strategy">The object strategy counterpart to accept or reject</param>
                                /// <param name="reason">And output reason for the rejection</param>
                                /// <returns>Whether the strategy is rejected or not</returns>
                                public override bool CanAttachStrategy(Dictionary<ObjectsManagementStrategy, bool> otherComponentsResults, ObjectStrategy strategy, ref string reason)
                                {
                                    if (!base.CanAttachStrategy(otherComponentsResults, strategy, ref reason)) return false;
                                    if (((BiomeObjectStrategy) strategy).biomeSet == biomeSet)
                                    {
                                        reason = "";
                                        return true;
                                    }
                                    reason = "Related object strategy counterpart uses a different biome set";
                                    return false;
                                }

                                public override bool CanAllocateMovement(
                                    Dictionary<ObjectsManagementStrategy, bool> otherComponentsResults, ObjectStrategy strategy,
                                    ObjectsManagementStrategyHolder.Status status, Direction direction,
                                    bool continued
                                )
                                {
                                    if (!otherComponentsResults[layoutObjectsManagementStrategy]) return false;
                                    // Also, if the object is NOT 1x1, then movement is currently not supported.
                                    // This will change in a future development, but so far it will be totally
                                    // ruled out.
                                    if (strategy.Object.Width != 1 || strategy.Object.Height != 1)
                                    {
                                        Debug.Log(
                                            "Currently, movement for non-1x1 objects is not supported in " +
                                            "the biome objects management strategy. This might be implemented in " +
                                            "the future, but so far is not.", strategy.Object
                                        );
                                        return false;
                                    }
                                    // Otherwise, the check will be done similarly to what the Layout strategy
                                    // does in general for block/unblock.
                                    return !IsAdjacencyUnset(biomes[((BiomeObjectStrategy)strategy).Biome], status.X, status.Y, strategy.StrategyHolder.Object.Width, strategy.StrategyHolder.Object.Height, direction);
                                }

                                public override bool CanClearMovement(
                                    Dictionary<ObjectsManagementStrategy, bool> otherComponentsResults,
                                    ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status)
                                {
                                    return true;
                                }
                                
                                /// <summary>
                                ///   Initializes an array of biomes, on each cell telling whether the biome
                                ///   is present or not.
                                /// </summary>
                                public override void InitGlobalCellsData()
                                {
                                    uint width = StrategyHolder.Map.Width;
                                    uint height = StrategyHolder.Map.Height;
                                    if (biomeSet == null)
                                    {
                                        throw new MissingBiomeSetException(
                                            "A biome set must be added to this management strategy"
                                        );
                                    }
                                    biomes = new Bitmask[biomeSet.Count];
                                    int l = biomes.Length;
                                    for (int i = 0; i < l; i++) biomes[i] = new Bitmask(width, height);
                                }

                                /// <summary>
                                ///   Computes the biome for a single cell, taking always the last
                                ///   biome setting according to all the tilemaps. Each biome in
                                ///   the array is set with the corresponding bit in the biome that
                                ///   is returned as the last (topmost) one for the cell.
                                /// </summary>
                                /// <param name="x">The x-position of the cell</param>
                                /// <param name="y">The y-position of the cell</param>
                                public override void ComputeCellData(uint x, uint y)
                                {
                                    ulong biome = 0;

                                    // First, get the last biome for the cell.
                                    foreach (UnityEngine.Tilemaps.Tilemap tilemap in StrategyHolder.Tilemaps)
                                    {
                                        UnityEngine.Tilemaps.TileBase tile = tilemap.GetTile(new Vector3Int((int)x, (int)y, 0));
                                        BiomeTileStrategy biomeTileStrategy = BundledTile.GetStrategyFrom<BiomeTileStrategy>(tile);
                                        if (biomeTileStrategy)
                                        {
                                            if (biomeTileStrategy.OverrideMode ==
                                                BiomeTileStrategy.BiomeOverrideMode.Extend)
                                            {
                                                biome |= biomeTileStrategy.BiomeMask;
                                            }
                                            else
                                            {
                                                biome = biomeTileStrategy.BiomeMask;
                                            }
                                        }
                                    }
                                    
                                    // Then, for each biome bit among the tolerated ones, set the mask.
                                    foreach (Bitmask bitmask in biomes)
                                    {
                                        bitmask.SetCell(x, y, (biome & 1) != 0);
                                        biome >>= 1;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
