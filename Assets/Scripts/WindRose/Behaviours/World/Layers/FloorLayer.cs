using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace WindRose
{
    namespace Behaviours
    {
        namespace World
        {
            namespace Layers
            {
                /**
                 * A floor layer will have several tilemaps.
                 */
                [RequireComponent(typeof(Grid))]
                public class FloorLayer : MapLayer
                {
                    protected override int GetSortingOrder()
                    {
                        return 0;
                    }

                    /**
                     * When starting, it will reset the transform of all its children tilemaps.
                     */
                    protected override void Start()
                    {
                        base.Start();
                        ForEachTilemap(delegate (UnityEngine.Tilemaps.Tilemap tilemap)
                        {
                            tilemap.transform.localPosition = Vector3.zero;
                            tilemap.transform.localRotation = Quaternion.identity;
                            tilemap.transform.localScale = Vector3.one;
                            return false;
                        });
                    }

                    /**
                     * Allows iterating over its tilemaps (perhaps to perform custom logic?).
                     */
                    public bool ForEachTilemap(Predicate<UnityEngine.Tilemaps.Tilemap> callback)
                    {
                        foreach (UnityEngine.Tilemaps.Tilemap tilemap in GetComponentsInChildren<UnityEngine.Tilemaps.Tilemap>())
                        {
                            if (callback(tilemap)) return true;
                        }
                        return false;
                    }
                }
            }
        }
    }
}
