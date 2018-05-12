using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        [RequireComponent(typeof(EventDispatcher))]
        [RequireComponent(typeof(BoxCollider2D))]
        public class TriggerPlatform : TriggerZone
        {
            /**
             * A TriggerPlatform is a strict WindRise component, and so will also correctly compute its
             *   collision mask, which will be a box.
             * 
             * However, despite being a box collider as well, its purpose will be complementary to
             *   TriggerActivator.
             */

            [SerializeField]
            private float innerMargin = 0.25f * Map.GAME_UNITS_PER_TILE_UNITS;

            protected override void Start()
            {
                base.Start();
                if (positionable.Solidness != Types.States.SolidnessStatus.Ghost && positionable.Solidness != Types.States.SolidnessStatus.Hole)
                {
                    positionable.SetSolidness(Types.States.SolidnessStatus.Ghost);
                }
            }

            protected override int GetDeltaX()
            {
                return (int)positionable.X;
            }

            protected override int GetDeltaY()
            {
                return (int)positionable.Y;
            }

            protected override EventDispatcher GetRelatedEventDispatcher()
            {
                return GetComponent<EventDispatcher>();
            }

            protected override Collider2D GetCollider2D()
            {
                return GetComponent<BoxCollider2D>();
            }

            protected override void SetupCollider(Collider2D collider2D)
            {
                BoxCollider2D boxCollider2D = (BoxCollider2D)collider2D;
                // collision mask will have certain width and height
                boxCollider2D.size = new Vector2(positionable.Width * Map.GAME_UNITS_PER_TILE_UNITS, positionable.Height * Map.GAME_UNITS_PER_TILE_UNITS);
                // and starting with those dimensions, we compute the offset as >>> and vvv
                boxCollider2D.offset = new Vector2(boxCollider2D.size.x / 2, -boxCollider2D.size.y / 2);
                // adjust to tolerate inner delta and avoid bleeding
                boxCollider2D.size = boxCollider2D.size - 2 * (new Vector2(innerMargin, innerMargin));
            }
        }
    }
}