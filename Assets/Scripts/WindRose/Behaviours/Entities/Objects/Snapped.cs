using UnityEngine;
using Support.Utils;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Entities.Objects
        {
            /// <summary>
            ///   <para>
            ///     Snaps the object's position vertically or horizontally to its logical
            ///       in-map position, and considering de cell's width and height.
            ///   </para>
            ///   <para>
            ///     When the object is moving, the movement axis will not be snapped.
            ///   </para>
            /// </summary>
            [RequireComponent(typeof(Object))]
            public class Snapped : MonoBehaviour
            {
                private Object mapObject;

                // Use this for initialization
                void Awake()
                {
                    mapObject = GetComponent<Object>();
                    mapObject.onAttached.AddListener(delegate (World.Map map)
                    {
                        // Forcing this to avoid a blink.
                        DoUpdate();
                    });
                }

                /// <summary>
                ///   <para>
                ///     This is a callback for the Update of the map object. It is
                ///       not intended to be called directly.
                ///   </para>
                ///   <para>
                ///     Clamps, snaps, and updates the position of the object in its
                ///       attached map.
                ///   </para>
                /// </summary>
                public void DoUpdate()
                {
                    // Run this code only if this object is attached to a map
                    if (mapObject.ParentMap == null) return;

                    bool snapInX = false;
                    bool snapInY = false;
                    bool clampInX = false;
                    bool clampInY = false;
                    float initialX = transform.localPosition.x;
                    // We invert the Y coordinate because States usually go up->down, and we expect it to be negative beforehand
                    float initialY = transform.localPosition.y;
                    float innerX = 0;
                    float innerY = 0;
                    float? minX = 0;
                    float? maxX = 0;
                    float? minY = 0;
                    float? maxY = 0;
                    float finalX = 0;
                    float finalY = 0;
                    float cellWidth = mapObject.GetCellWidth();
                    float cellHeight = mapObject.GetCellHeight();

                    // In this context, we can ALWAYS check for its current movement or position.

                    switch (mapObject.Movement)
                    {
                        case Types.Direction.LEFT:
                            snapInY = true;
                            clampInX = true;
                            minX = null;
                            maxX = mapObject.X * cellWidth;
                            break;
                        case Types.Direction.RIGHT:
                            snapInY = true;
                            clampInX = true;
                            minX = mapObject.X * cellWidth;
                            maxX = null;
                            break;
                        case Types.Direction.UP:
                            snapInX = true;
                            clampInY = true;
                            minY = mapObject.Y * cellHeight;
                            maxY = null;
                            break;
                        case Types.Direction.DOWN:
                            snapInX = true;
                            clampInY = true;
                            minY = null;
                            maxY = mapObject.Y * cellHeight;
                            break;
                        default:
                            snapInX = true;
                            snapInY = true;
                            break;
                    }

                    innerX = snapInX ? mapObject.X * cellWidth : initialX;
                    innerY = snapInY ? mapObject.Y * cellHeight : initialY;

                    finalX = clampInX ? Values.Clamp<float>(minX, innerX, maxX) : innerX;
                    finalY = clampInY ? Values.Clamp<float>(minY, innerY, maxY) : innerY;

                    // We make the Y coordinate negative, as it was (or should be) in the beginning.
                    transform.localPosition = new Vector3(finalX, finalY, transform.localPosition.z);
                }
            }
        }
    }
}