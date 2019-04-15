using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Entities.Objects
        {
            /// <summary>
            ///   A platform notifies the zone events (see <see cref="TriggerZone"/> for more details)
            ///     and calculates the zone bounds based on its underlying map object's dimensions,
            ///     but considering an "inner margin" factor to avoid collisions when objects are
            ///     not there but immediately adjacent in any axis.
            /// </summary>
            [RequireComponent(typeof(MapObject))]
            [RequireComponent(typeof(BoxCollider2D))]
            public class TriggerPlatform : TriggerZone
            {
                /// <summary>
                ///   The inner margin to set. It must be strictly positive to avoid "bleeding"
                ///     (collisions with adjacent <see cref="TriggerLive"/> objects).
                /// </summary>
                [SerializeField]
                private float innerMarginFactor = 0.25f;

                /// <summary>
                ///   The delta X is the object's X position.
                /// </summary>
                /// <returns>The delta X</returns>
                protected override int GetDeltaX()
                {
                    return (int)mapObject.X;
                }

                /// <summary>
                ///   The delta Y is the object's Y position.
                /// </summary>
                /// <returns>The delta Y</returns>
                protected override int GetDeltaY()
                {
                    return (int)mapObject.Y;
                }

                /// <summary>
                ///   The related map object is itself.
                /// </summary>
                /// <returns></returns>
                protected override MapObject GetRelatedObject()
                {
                    return GetComponent<MapObject>();
                }

                /// <summary>
                ///   The related collider is its <see cref="BoxCollider2D"/>
                ///     component, required right here.
                /// </summary>
                /// <returns></returns>
                protected override Collider2D GetCollider2D()
                {
                    return GetComponent<BoxCollider2D>();
                }

                /// <summary>
                ///   Sets up the collider considering not just its dimensions
                ///     (like <see cref="TriggerLive"/> does) but also the inner
                ///     margin to avoid bleeding.
                /// </summary>
                /// <param name="collider2D">The collider to set up</param>
                protected override void SetupCollider(Collider2D collider2D)
                {
                    BoxCollider2D boxCollider2D = (BoxCollider2D)collider2D;
                    float cellWidth = mapObject.GetCellWidth();
                    float cellHeight = mapObject.GetCellHeight();
                    // collision mask will have certain width and height
                    boxCollider2D.size = new Vector2(mapObject.Width * cellWidth, mapObject.Height * cellHeight);
                    // and starting with those dimensions, we compute the offset as >>> and vvv
                    boxCollider2D.offset = new Vector2(boxCollider2D.size.x / 2, boxCollider2D.size.y / 2);
                    // adjust to tolerate inner delta and avoid bleeding
                    boxCollider2D.size = boxCollider2D.size - 2 * (new Vector2(innerMarginFactor * cellWidth, innerMarginFactor * cellHeight));
                }
            }
        }
    }
}