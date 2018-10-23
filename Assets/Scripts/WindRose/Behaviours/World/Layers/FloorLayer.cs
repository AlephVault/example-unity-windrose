using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering;

namespace WindRose
{
    namespace Behaviours
    {
        namespace World
        {
            namespace Layers
            {
                [RequireComponent(typeof(Grid))]
                public class FloorLayer : MapLayer
                {
                    /**
                     * A floor layer will have several floors, which are in turn tilemaps.
                     * It will scrap its tilemaps, however.
                     */

                    private Tilemap[] tilemaps;

                    private class TempListElement
                    {
                        public readonly int SortingOrder;
                        public readonly TilemapRenderer Renderer;
                        public readonly Tilemap Tilemap;

                        public TempListElement(int sortingOrder, TilemapRenderer renderer, Tilemap tilemap)
                        {
                            SortingOrder = sortingOrder;
                            Renderer = renderer;
                            Tilemap = tilemap;
                        }
                    }

                    protected override void Awake()
                    {
                        base.Awake();
                        // We sort the layers accordingly - please use different sorting orders explicitly.
                        List<TempListElement> elements = new List<TempListElement>();
                        foreach (Floors.Floor floor in GetComponentsInChildren<Floors.Floor>())
                        {
                            TilemapRenderer renderer = floor.GetComponent<TilemapRenderer>();
                            elements.Add(new TempListElement(renderer.sortingOrder, renderer, floor.GetComponent<Tilemap>()));
                        }
                        tilemaps = (from element in elements
                                    orderby element.SortingOrder
                                    select element.Tilemap).ToArray();
                    }

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
                        int index = 0;
                        ForEachTilemap(delegate (Tilemap tilemap)
                        {
                            TilemapRenderer renderer = tilemap.GetComponent<TilemapRenderer>();
                            renderer.sortingLayerID = 0;
                            renderer.sortingOrder = index++;
                            return false;
                        });
                    }

                    /**
                     * Allows iterating over its tilemaps (perhaps to perform custom logic?).
                     */
                    public bool ForEachTilemap(Predicate<Tilemap> callback)
                    {
                        foreach (Tilemap tilemap in tilemaps)
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
