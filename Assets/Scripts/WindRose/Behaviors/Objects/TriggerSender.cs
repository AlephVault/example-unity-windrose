using System;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        [RequireComponent(typeof(Rigidbody2D))]
        [RequireComponent(typeof(EventDispatcher))]
        [RequireComponent(typeof(TriggerEnabled))]
        public class TriggerSender : MonoBehaviour
        {
            /**
             * This helps us acting as a sending trigger. WindRose events dispatches will
             *   be relevant in this behaviour. These events will be used from the counterpart
             *   (i.e. the Trigger Receiver).
             * 
             * A rigidbody will be needed and will be turned kinematic. Otherwise, triggers will
             *   not work.
             * 
             * A TriggerEnabled will be needed so the collision mask can be set accordingly.
             */

            void Start()
            {
                Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
                rigidbody.isKinematic = true;
            }
        }
    }
}