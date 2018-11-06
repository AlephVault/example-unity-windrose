using UnityEngine;
using Support.Utils;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Objects
        {
            [RequireComponent(typeof(Positionable))]
            public class Snapped : MonoBehaviour
            {
                /**
                 * A snapped object provides behavior to react to its position
                 *   (since it will also be a positionable object).
                 * 
                 * In order to map its Positionable's position against the position
                 *   in the scene, we must define constants to map between Positionable's
                 *   units and Scene's units. The constant SCENE_UNITS_PER_TILE_UNITS
                 *   holds that ratio, and if you edit it, at least keep your game
                 *   design as consistent as possible among scene and sprites.
                 */

                private Positionable positionable;

                // Use this for initialization
                void Awake()
                {
                    positionable = GetComponent<Positionable>();
                    EventDispatcher dispatcher = GetComponent<EventDispatcher>();
                    dispatcher.onAttached.AddListener(delegate (World.Map map)
                    {
                        // Forcing this to avoid a blink.
                        Update();
                    });
                }

                // Update is called once per frame
                void Update()
                {
                    // Run this code only if this object is attached to a map
                    if (positionable.ParentMap == null) return;

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
                    float cellWidth = positionable.GetCellWidth();
                    float cellHeight = positionable.GetCellHeight();

                    // A positionable will ALWAYS be attached, since Start, until Destroy.
                    // In this context, we can ALWAYS check for its current movement or position.

                    switch (positionable.Movement)
                    {
                        case Types.Direction.LEFT:
                            snapInY = true;
                            clampInX = true;
                            minX = null;
                            maxX = positionable.X * cellWidth;
                            break;
                        case Types.Direction.RIGHT:
                            snapInY = true;
                            clampInX = true;
                            minX = positionable.X * cellWidth;
                            maxX = null;
                            break;
                        case Types.Direction.UP:
                            snapInX = true;
                            clampInY = true;
                            minY = positionable.Y * cellHeight;
                            maxY = null;
                            break;
                        case Types.Direction.DOWN:
                            snapInX = true;
                            clampInY = true;
                            minY = null;
                            maxY = positionable.Y * cellHeight;
                            break;
                        default:
                            snapInX = true;
                            snapInY = true;
                            break;
                    }

                    innerX = snapInX ? positionable.X * cellWidth : initialX;
                    innerY = snapInY ? positionable.Y * cellHeight : initialY;

                    finalX = clampInX ? Values.Clamp<float>(minX, innerX, maxX) : innerX;
                    finalY = clampInY ? Values.Clamp<float>(minY, innerY, maxY) : innerY;

                    // We make the Y coordinate negative, as it was (or should be) in the beginning.
                    transform.localPosition = new Vector3(finalX, finalY, transform.localPosition.z);
                }
            }
        }
    }
}