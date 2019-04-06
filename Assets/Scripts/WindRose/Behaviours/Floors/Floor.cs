﻿using UnityEngine;
using UnityEngine.Tilemaps;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Floors
        {
            /// <summary>
            ///   A floor is a behaviour that normalizes the position of a tilemap inside a map.
            ///   Floors will be identified from object management strategies (and strategy holders)
            ///     to get data from their tiles.
            ///   Many floors may exist (and, often, will) in a single map. They will be stacked
            ///     appropriately and, since they have a <see cref="TilemapRenderer"/>, they may be
            ///     given any sort order of choice.
            /// </summary>
            [RequireComponent(typeof(Tilemap))]
            [RequireComponent(typeof(TilemapRenderer))]
            [RequireComponent(typeof(Support.Behaviours.Normalized))]
            class Floor : MonoBehaviour
            {
                /// <summary>
                ///   Tells when the parent is not a <see cref="World.Layers.Floor.FloorLayer"/>.
                /// </summary>
                public class ParentMustBeFloorLayerException : Types.Exception
                {
                    public ParentMustBeFloorLayerException() : base() { }
                    public ParentMustBeFloorLayerException(string message) : base(message) { }
                }

                private void Awake()
                {
                    try
                    {
                        Support.Utils.Layout.RequireComponentInParent<World.Layers.Floor.FloorLayer>(this);
                        // TODO in version 2018.x+ I have to require RECTANGULAR tilemap, or explode.
                        Tilemap tilemap = GetComponent<Tilemap>();
                        tilemap.orientation = Tilemap.Orientation.XY;
                        TilemapRenderer tilemapRenderer = GetComponent<TilemapRenderer>();
                        tilemapRenderer.sortOrder = TilemapRenderer.SortOrder.BottomLeft;
                    }
                    catch (Types.Exception)
                    {
                        Destroy(gameObject);
                        throw new ParentMustBeFloorLayerException();
                    }
                }
            }
        }
    }
}
