using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace WindRose
{
    namespace MenuActions
    {
        namespace Tiles
        {
            static class TileUtils
            {
                public static IEnumerable<T> GetSelectedAssets<T>() where T : Object
                {
                    foreach(string assetGUID in Selection.assetGUIDs)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(assetGUID);
                        T asset = AssetDatabase.LoadAssetAtPath<T>(path);
                        if (asset != null)
                            yield return asset; 
                    }
                }
            }
        }
    }
}
