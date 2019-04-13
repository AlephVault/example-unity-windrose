using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

namespace WindRose
{
    namespace MenuActions
    {
        namespace Tiles
        {
            using Support.Utils;

            public static class StandardTileUtils
            {
                [MenuItem("Assets/Create/Wind Rose/Tiles/Tiles (From 1+ selected sprites)", false, priority = 101)]
                public static void CreateTiles()
                {
                    Sprite[] sprites = TileUtils.GetSelectedAssets<Sprite>().ToArray();
                }

                [MenuItem("Assets/Create/Wind Rose/Tiles/Tiles (From 1+ selected sprites)", true)]
                public static bool CanCreateTiles()
                {
                    return TileUtils.GetSelectedAssets<Sprite>().Count() >= 1;
                }

                [MenuItem("Assets/Create/Wind Rose/Tiles/Random Tile (From 2+ selected sprites)", false, priority = 102)]
                public static void CreateRandomTile()
                {
                    Sprite[] sprites = TileUtils.GetSelectedAssets<Sprite>().ToArray();
                    string path = AssetDatabase.GetAssetPath(sprites[0].GetInstanceID());
                    string parentPath = Path.GetDirectoryName(path);
                    string fileName = Path.ChangeExtension(Path.GetFileName(path), "asset");
                    string bundledPath = Path.Combine(parentPath, "Tiles");
                    if (!AssetDatabase.IsValidFolder(bundledPath))
                    {
                        AssetDatabase.CreateFolder(parentPath, "Tiles");
                    }
                    RandomTile randomTile = ScriptableObject.CreateInstance<RandomTile>();
                    Layout.SetObjectFieldValues(randomTile, new Dictionary<string, object>() {
                        { "m_Sprites", sprites }
                    });
                    AssetDatabase.CreateAsset(randomTile, Path.Combine(bundledPath, fileName));
                }

                [MenuItem("Assets/Create/Wind Rose/Tiles/Random Tile (From 2+ selected sprites)", true)]
                public static bool CanCreateRandomTile()
                {
                    return TileUtils.GetSelectedAssets<Sprite>().Count() >= 2;
                }
            }
        }
    }
}
