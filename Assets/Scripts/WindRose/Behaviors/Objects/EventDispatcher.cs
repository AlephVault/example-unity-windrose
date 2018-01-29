using System;
using UnityEngine;
using UnityEngine.Events;

namespace WindRose
{
    namespace Behaviours
    {
        [RequireComponent(typeof(Positionable))]
        public class EventDispatcher : MonoBehaviour
        {
            /**
             * An event dispatcher object receives the internal WindRose events
             *   and allows the users (external objects) to install/remove
             *   interactions/listenings from events this object triggers.
             *   
             * Usually, internal WindRose events are just internal to these kind
             *   of individual Positionable-related behaviours. Those methods
             *   should remain... internal. This behaviour is intended to
             *   provide external access
             */

            [Serializable]
            public class UnityAttachedEvent : UnityEvent<Map> {}
            public readonly UnityAttachedEvent onAttached = new UnityAttachedEvent();
            public readonly UnityEvent onDetached = new UnityEvent();
            [Serializable]
            public class UnityMovementEvent : UnityEvent<Types.Direction> {}
            public readonly UnityMovementEvent onMovementStarted = new UnityMovementEvent();
            public readonly UnityMovementEvent onMovementCancelled = new UnityMovementEvent();
            public readonly UnityMovementEvent onMovementFinished = new UnityMovementEvent();
            [Serializable]
            public class UnitySolidnessStatusEvent : UnityEvent<Types.Tilemaps.SolidnessStatus> {}
            public readonly UnitySolidnessStatusEvent onSolidnessChanged = new UnitySolidnessStatusEvent();
            [Serializable]
            public class UnityTeleportedEvent : UnityEvent<uint, uint> {}
            public readonly UnityTeleportedEvent onTeleported = new UnityTeleportedEvent();

            void OnAttached(object[] args)
            {
                onAttached.Invoke(((Types.Tilemaps.Tilemap)(args[0])).RelatedMap);
            }

            void OnDetached()
            {
                onDetached.Invoke();
            }

            void OnMovementStarted(object[] args)
            {
                onMovementStarted.Invoke((Types.Direction)(args[0]));
            }

            void OnMovementCancelled(object[] args)
            {
                onMovementCancelled.Invoke((Types.Direction)(args[0]));
            }

            void OnMovementFinished(object[] args)
            {
                onMovementFinished.Invoke((Types.Direction)(args[0]));
            }

            void OnSolidnessChanged(object[] args)
            {
                onSolidnessChanged.Invoke((Types.Tilemaps.SolidnessStatus)(args[0]));
            }

            void OnTeleported(object[] args)
            {
                onTeleported.Invoke((uint)(args[0]), (uint)(args[1]));
            }
        }
    }
}