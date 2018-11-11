using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WindRose
{
    namespace ScriptableObjects
    {
        namespace Tiles
        {
            [CreateAssetMenu(fileName = "NewBundledTile", menuName = "Wind Rose/Tiles/Bundled Tile", order = 201)]
            public class BundledTile : TileBase
            {
                /**
                 * This tile is a bundle of strategies. It does nothing
                 *   by itself, but depends on another (non-strategy) tile.
                 *   However, it contains several data bundles (called
                 *   strategies, although they have no behaviour a priori
                 *   but just data).
                 *   
                 * The tile has behaviour to retrieve those strategy instances,
                 *   and the instances can be edited via its editor.
                 */
                [SerializeField]
                private TileBase sourceTile;

                [SerializeField]
                private Strategies.TileStrategy[] strategies;

                public class TileStrategyDependencyException : Support.Utils.AssetsLayout.DependencyException
                {
                    public TileStrategyDependencyException(string message) : base(message) {}
                }

                void Awake()
                {
                    try
                    {
                        // Order / Flatten dependencies
                        strategies = Support.Utils.AssetsLayout.FlattenDependencies<Strategies.TileStrategy, RequireTileStrategy, TileStrategyDependencyException>(strategies);
                    }
                    catch(Exception)
                    {
                        Resources.UnloadAsset(this);
                    }
                }

                public override bool GetTileAnimationData(Vector3Int position, ITilemap tilemap, ref TileAnimationData tileAnimationData)
                {
                    return sourceTile.GetTileAnimationData(position, tilemap, ref tileAnimationData);
                }

                public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
                {
                    sourceTile.GetTileData(position, tilemap, ref tileData);
                }

                public override void RefreshTile(Vector3Int position, ITilemap tilemap)
                {
                    sourceTile.RefreshTile(position, tilemap);
                }

                public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
                {
                    return sourceTile.StartUp(position, tilemap, go);
                }

                public T GetStrategy<T>() where T : Strategies.TileStrategy
                {
                    return (from strategy in strategies where strategy is T select (T)strategy).FirstOrDefault();
                }

                public T[] GetStrategies<T>() where T : Strategies.TileStrategy
                {
                    return (from strategy in strategies where strategy is T select (T)strategy).ToArray();
                }

                /**
                 * These two static helpers allow us to retrieve the appropriate components from tiles.
                 * If the given tiles are not BundledTile objects, then null / empty array are returned.
                 * Otherwise, the call is delegated to the respective instance methods in the tile.
                 */

                public static T GetStrategyFrom<T>(TileBase tile) where T : Strategies.TileStrategy
                {
                    if (tile is BundledTile) {
                        return ((BundledTile)tile).GetStrategy<T>();
                    } else {
                        return null;
                    }
                }

                public static T[] GetStrategiesFrom<T>(TileBase tile) where T : Strategies.TileStrategy
                {
                    return (tile is BundledTile) ? ((BundledTile)tile).GetStrategies<T>() : (new T[0]);
                }
            }
        }
    }
}
