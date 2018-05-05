using System;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        [RequireComponent(typeof(Rigidbody2D))]
        [RequireComponent(typeof(EventDispatcher))]
        [RequireComponent(typeof(BoxCollider2D))]
        public abstract class TriggerLive : TriggerHolder
        {
            /**
             * This abstract trigger has only the task of being a kinematic rigidbody, which is
             *   required to trigger with subclasses from TriggerZone. This abstract class should
             *   be a complement of TriggerZone.
             * 
             * Live triggers are deeply tied to WindRose, since they interact with event dispatchers
             *   which are in turn positionables: Live triggers are live map object, which could be
             *   not the case for other types of colliders.
             */

            private Rigidbody2D rigidbody2D;
            protected override void Awake()
            {
                base.Awake();
                rigidbody2D = GetComponent<Rigidbody2D>();
            }

            protected override Collider2D GetCollider2D()
            {
                return GetComponent<BoxCollider2D>();
            }

            protected override void Start()
            {
                base.Start();
                rigidbody2D.isKinematic = true;
            }
        }
    }
}