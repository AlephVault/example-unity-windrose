using System;
using UnityEngine;
using UnityEngine.Events;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Objects
        {
            using World;

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
                public class UnityAttachedEvent : UnityEvent<Map> { }
                public readonly UnityAttachedEvent onAttached = new UnityAttachedEvent();
                public readonly UnityEvent onDetached = new UnityEvent();
                [Serializable]
                public class UnityMovementEvent : UnityEvent<Types.Direction> { }
                [Serializable]
                public class UnityOptionalMovementEvent : UnityEvent<Types.Direction?> { }
                public readonly UnityMovementEvent onMovementStarted = new UnityMovementEvent();
                public readonly UnityOptionalMovementEvent onMovementCancelled = new UnityOptionalMovementEvent();
                public readonly UnityMovementEvent onMovementFinished = new UnityMovementEvent();
                [Serializable]
                public class UnityPropertyUpdateEvent : UnityEvent<string, object, object> { }
                public readonly UnityPropertyUpdateEvent onPropertyUpdated = new UnityPropertyUpdateEvent();
                [Serializable]
                public class UnityTeleportedEvent : UnityEvent<uint, uint> { }
                public readonly UnityTeleportedEvent onTeleported = new UnityTeleportedEvent();

                void OnDestroy()
                {
                    onAttached.RemoveAllListeners();
                    onDetached.RemoveAllListeners();
                    onMovementStarted.RemoveAllListeners();
                    onMovementCancelled.RemoveAllListeners();
                    onMovementFinished.RemoveAllListeners();
                    onPropertyUpdated.RemoveAllListeners();
                    onTeleported.RemoveAllListeners();
                }
            }
        }
    }
}