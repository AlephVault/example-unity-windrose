using System;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        [RequireComponent(typeof(Positionable))]
        [RequireComponent(typeof(BoxCollider2D))]
        public class TriggerEnabled : MonoBehaviour
        {
            /**
             * This behaviour configures a collision mask in the collider2d
             *   component, and turns it into a trigger.
             * 
             * The size and pivot are determined once, and this behaviour does
             *   nothing else regarding the collisions, like trying to detect
             *   them or handling events.
             * 
             * This works regardless the object has a rigidbody or not, since this
             *   only sets the size and position of the box collider.
             */

            [SerializeField]
            private float innerMargin = 0.25f;

            void Start()
            {
                Positionable positionable = GetComponent<Positionable>();
                BoxCollider2D collider2D = GetComponent<BoxCollider2D>();
                // collision will be a trigger
                collider2D.isTrigger = true;
                // collision mask will have certain width and height
                collider2D.size = new Vector2(positionable.Width * Map.GAME_UNITS_PER_TILE_UNITS, positionable.Height * Map.GAME_UNITS_PER_TILE_UNITS);
                // and starting with those dimensions, we compute the offset as >>> and vvv
                collider2D.offset = new Vector2(collider2D.size.x / 2, -collider2D.size.y / 2);
                // adjust to tolerate inner delta and avoid bleeding
                collider2D.size = collider2D.size - 2 * (new Vector2(innerMargin, innerMargin));
            }
        }
    }
}