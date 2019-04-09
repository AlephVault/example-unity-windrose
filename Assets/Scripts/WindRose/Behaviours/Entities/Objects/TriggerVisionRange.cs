﻿using System;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Entities.Objects
        {
            /// <summary>
            ///   <para>
            ///     Vision ranges are related to <see cref="Watcher"/> objects. They have a way to
            ///       tell their own dimensions but are strictly tied to such watcher objects, who
            ///       receive all the events. Think of the regions in Pokemon games where you step
            ///       and a trainer spots you and a fight starts.
            ///   </para>
            ///   <para>
            ///     Usually, this object is related to watchers, and nothing needs to be done here.
            ///       However, this component can be created on its own, provided the
            ///       <see cref="relatedObject"/> is filled accordingly.
            ///   </para>
            ///   <para>
            ///     Vision ranges spread to certain direction (being specified or being taken from
            ///       the related map object's <see cref="Oriented"/> component), with a given
            ///       length (considering a base of 1), and a given width (considering a base
            ///       of 1, and spreading to each side of the main, oriented, spread).
            ///   </para>
            /// </summary>
            [RequireComponent(typeof(BoxCollider2D))]
            public class TriggerVisionRange : TriggerZone
            {
                // This inner margin is not mutable and will work to avoid bleeding
                const float BLEEDING_BUFFER = 0.1f;

                /// <summary>
                ///   The related map object. It is mandatory.
                /// </summary>
                [SerializeField]
                private Object relatedObject;

                // Perhaps the related object has an Oriented component. We will make use of it.
                private Oriented oriented;

                /// <summary>
                ///   If the related map object has an <see cref="Oriented"/> component, this
                ///     property is meaningless. However, if it doesn't, then this property
                ///     tells which dimensions this vision range spreads to.
                /// </summary>
                [SerializeField]
                private Types.Direction direction = Types.Direction.DOWN;

                /// <summary>
                ///   Size corresponds to half-width, rounded down. e.g. 0 corresponds
                ///     to 1-cell width, 1 corresponds to 3-cell width, 3 to 5, ...
                /// </summary>
                [SerializeField]
                private uint visionSize = 0;

                /// <summary>
                ///   Length corresponds to how far does the vision reach. The actual length will be this
                ///     value, plus 1 (the immediately next step is not counted as part of the vision
                ///     range).
                /// </summary>
                [SerializeField]
                private uint visionLength = 0;

                private uint halfWidth;
                private uint halfHeight;

                private void OrientationChanged(Types.Direction orientation)
                {
                    if (mapObject.ParentMap) RefreshDimensions();
                }

                protected override void Awake()
                {
                    base.Awake();
                    oriented = mapObject.GetComponent<Oriented>();
                    if (mapObject.Width % 2 == 0 || mapObject.Height % 2 == 0)
                    {
                        Destroy(gameObject);
                        throw new Types.Exception("For a vision range to work appropriately, the related map object must have an odd width and height");
                    }
                    halfHeight = mapObject.Height / 2;
                    halfWidth = mapObject.Width / 2;
                    if (oriented) oriented.onOrientationChanged.AddListener(OrientationChanged);
                }

                private void OnDestroy()
                {
                    if (oriented)
                    {
                        oriented.onOrientationChanged.RemoveListener(OrientationChanged);
                    }
                }

                protected override void Start()
                {
                    base.Start();
                    // Forcing accurate position the first time
                    if (mapObject.ParentMap) Update();
                }

                protected override int GetDeltaX()
                {
                    int extra;
                    switch (direction)
                    {
                        case Types.Direction.RIGHT:
                            extra = (int)mapObject.Width;
                            break;
                        case Types.Direction.UP:
                        case Types.Direction.DOWN:
                            extra = (int)halfWidth;
                            break;
                        default:
                            extra = 0;
                            break;
                    }
                    return extra + (int)mapObject.X;
                }

                protected override int GetDeltaY()
                {
                    int extra;
                    switch (direction)
                    {
                        case Types.Direction.UP:
                            extra = (int)mapObject.Height;
                            break;
                        case Types.Direction.LEFT:
                        case Types.Direction.RIGHT:
                            extra = (int)halfHeight;
                            break;
                        default:
                            extra = 0;
                            break;
                    }
                    return extra + (int)mapObject.Y;
                }

                /// <summary>
                ///   The related map object is the specified in <see cref="relatedObject"/>.
                /// </summary>
                /// <returns>The related map object</returns>
                protected override Object GetRelatedObject()
                {
                    return relatedObject;
                }

                /// <summary>
                ///   The involved collider is the required <see cref="BoxCollider"/>.
                /// </summary>
                /// <returns>The involved collider</returns>
                protected override Collider2D GetCollider2D()
                {
                    return GetComponent<BoxCollider2D>();
                }

                /// <summary>
                ///   The collider is set up on every orientation change, involving
                ///     the central and side spread (in <see cref="visionLength"/> and
                ///     <see cref="visionSize"/>), and the bleeding.
                /// </summary>
                /// <param name="collider2D">The collider to with with</param>
                protected override void SetupCollider(Collider2D collider2D)
                {
                    BoxCollider2D boxCollider2D = (BoxCollider2D)collider2D;
                    float cellWidth = mapObject.GetCellWidth();
                    float cellHeight = mapObject.GetCellHeight();
                    // we set the size based on the direction we are looking, and also the offset back to the right-top corner
                    switch (direction)
                    {
                        case Types.Direction.UP:
                        case Types.Direction.DOWN:
                            boxCollider2D.size = new Vector2((visionSize * 2 + 1) * cellWidth, (visionLength + 1) * cellHeight);
                            break;
                        default:
                            boxCollider2D.size = new Vector2((visionLength + 1) * cellWidth, (visionSize * 2 + 1) * cellHeight);
                            break;
                    }
                    boxCollider2D.offset = new Vector2(0.5f * boxCollider2D.size.x, 0.5f * boxCollider2D.size.y);
                    // also we set the transform of this vision range, using global coordinates:
                    Vector3 basePosition = mapObject.transform.position;
                    Vector3 newPosition = Vector3.zero;
                    switch (direction)
                    {
                        case Types.Direction.UP:
                            newPosition = new Vector3(basePosition.x - visionSize * cellWidth, basePosition.y + mapObject.Height * cellHeight, basePosition.z);
                            break;
                        case Types.Direction.DOWN:
                            newPosition = new Vector3(basePosition.x - visionSize * cellWidth, basePosition.y - (visionLength + 1) * cellHeight, basePosition.z);
                            break;
                        case Types.Direction.LEFT:
                            newPosition = new Vector3(basePosition.x - boxCollider2D.size.x, basePosition.y - visionSize * cellHeight, basePosition.z);
                            break;
                        case Types.Direction.RIGHT:
                            newPosition = new Vector3(basePosition.x + mapObject.Width * cellWidth, basePosition.y - visionSize * cellHeight, basePosition.z);
                            break;
                        default:
                            break;
                    }
                    transform.position = newPosition;
                    // We apply the bleeding buffer right here
                    boxCollider2D.size = boxCollider2D.size - 2 * new Vector2(BLEEDING_BUFFER * cellWidth, BLEEDING_BUFFER * cellHeight);
                }
            }
        }
    }
}