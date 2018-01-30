using System;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        [RequireComponent(typeof(Rigidbody2D))]
        [RequireComponent(typeof(BoxCollider2D))]
        public class TriggerActivator : TriggerHolder
        {
            /**
             * This helps us acting as a sending trigger. WindRose events dispatches will
             *   be relevant in this behaviour. These events will be used from the counterpart
             *   (i.e. the Trigger Receiver).
             * 
             * A rigidbody will be needed and will be turned kinematic. Otherwise, triggers will
             *   not work.
             * 
             * This activator is strict part of WindRose objects and so will only consider
             *   BoxCollider2D as available collider to be used. Also it will determine its dimensions
             *   according to its positionable's Width and Height.
             */

            private Positionable positionable;
            private Rigidbody2D rigidbody2D;
            protected override void Awake()
            {
                base.Awake();
                positionable = GetComponent<Positionable>();
                rigidbody2D = GetComponent<Rigidbody2D>();
            }

            protected override void Start()
            {
                base.Start();
                rigidbody2D.isKinematic = true;
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
            }
        }
    }
}