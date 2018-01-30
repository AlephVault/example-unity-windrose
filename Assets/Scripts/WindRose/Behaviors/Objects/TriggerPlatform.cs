using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WindRose
{
    namespace Behaviours
    {
        [RequireComponent(typeof(Positionable))]
        [RequireComponent(typeof(BoxCollider2D))]
        public class TriggerPlatform : TriggerZone
        {
            /**
             * A TriggerPlatform is a strict WindRise component, and so will also correctly compute its
             *   collision mask, which will be a box.
             * 
             * However, despite being a box collider as well, its purpose will be complementary to
             *   TriggerActivator.
             * 
             * This component will do/ensure/detect the following behaviour on each TriggerActivator:
             *   1. A trigger activator has just fulfilled two conditions:
             *      i. be in the same map as this trigger receiver.
             *      ii. staying inside this trigger receiver.
             *   2. A trigger activator has just fulfilled one or more of these conditions:
             *      i. be in different map.
             *      ii. getting out of this trigger platform.
             *   3. A trigger activator is inside this map, and this trigger platform.
             *   4. OnDestroy will clear all event callbacks on all currently staying TriggerActivators.
             *   5. The installed callback will attend the "it moved!" event.
             */
                        
            private Positionable positionable;

            // These five events are notified against the involved Positionable components of
            //   already registered TriggerSender objects, the positionable of this object,
            //   and the delta coordinates between them.
            [Serializable]
            public class UnityMapTriggerEvent : UnityEvent<Positionable, Positionable, int, int> {}
            public readonly UnityMapTriggerEvent onMapTriggerEnter = new UnityMapTriggerEvent();
            public readonly UnityMapTriggerEvent onMapTriggerStay = new UnityMapTriggerEvent();
            public readonly UnityMapTriggerEvent onMapTriggerExit = new UnityMapTriggerEvent();
            public readonly UnityMapTriggerEvent onMapTriggerMoved = new UnityMapTriggerEvent();

            [SerializeField]
            private float innerMargin = 0.25f;

            private void InvokeEventCallback(Positionable senderObject, UnityMapTriggerEvent targetEvent)
            {
                targetEvent.Invoke(senderObject, positionable, (int)senderObject.X - (int)positionable.X, (int)senderObject.Y - (int)positionable.Y);
            }

            protected override void CallOnMapTriggerEnter(Positionable senderObject)
            {
                InvokeEventCallback(senderObject, onMapTriggerEnter);
            }

            protected override void CallOnMapTriggerStay(Positionable senderObject)
            {
                InvokeEventCallback(senderObject, onMapTriggerStay);
            }

            protected override void CallOnMapTriggerExit(Positionable senderObject)
            {
                InvokeEventCallback(senderObject, onMapTriggerExit);
            }

            protected override void CallOnMapTriggerPositionChanged(Positionable senderObject)
            {
                InvokeEventCallback(senderObject, onMapTriggerMoved);
            }

            protected override void Awake()
            {
                base.Awake();
                positionable = GetComponent<Positionable>();
            }

            protected override void Start()
            {
                base.Start();
                if (positionable.Solidness != Types.Tilemaps.SolidnessStatus.Ghost && positionable.Solidness != Types.Tilemaps.SolidnessStatus.Hole)
                {
                    positionable.SetSolidness(Types.Tilemaps.SolidnessStatus.Ghost);
                }
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