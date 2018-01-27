using System;
using UnityEngine;

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

            private Action<Map> onAttached = (_) => {};
            private Action onDetached = () => {};
            private Action<Types.Direction> onMovementStarted = (_) => {};
            private Action<Types.Direction> onMovementCancelled = (_) => {};
            private Action<Types.Direction> onMovementFinished = (_) => {};
            private Action<Types.Tilemaps.SolidnessStatus> onSolidnessChanged = (_) => {};
            private Action<uint, uint> onTeleported = (x, y) => {};

            public void AddOnAttachedListener(Action<Map> action)
            {
                onAttached += action;
            }

            public void AddOnDetachedListener(Action action)
            {
                onDetached += action;
            }

            public void AddOnMovementStartedListener(Action<Types.Direction> action)
            {
                onMovementStarted += action;
            }

            public void AddOnMovementCancelledListener(Action<Types.Direction> action)
            {
                onMovementCancelled += action;
            }

            public void AddOnMovementFinishedListener(Action<Types.Direction> action)
            {
                onMovementFinished += action;
            }

            public void AddOnSolidnessChangedListener(Action<Types.Tilemaps.SolidnessStatus> action)
            {
                onSolidnessChanged += action;
            }

            public void AddOnTeleportedListener(Action<uint, uint> action)
            {
                onTeleported += action;
            }

            public void RemoveOnAttachedListener(Action<Map> action)
            {
                onAttached -= action;
            }

            public void RemoveOnDetachedListener(Action action)
            {
                onDetached -= action;
            }

            public void RemoveOnMovementStartedListener(Action<Types.Direction> action)
            {
                onMovementStarted -= action;
            }

            public void RemoveOnMovementCancelledListener(Action<Types.Direction> action)
            {
                onMovementCancelled -= action;
            }

            public void RemoveOnMovementFinishedListener(Action<Types.Direction> action)
            {
                onMovementFinished -= action;
            }

            public void RemoveOnSolidnessChangedListener(Action<Types.Tilemaps.SolidnessStatus> action)
            {
                onSolidnessChanged -= action;
            }

            public void RemoveOnTeleportedListener(Action<uint, uint> action)
            {
                onTeleported -= action;
            }

            void OnAttached(object[] args)
            {
                onAttached((Map)(((Types.Tilemaps.Tilemap)(args[0])).RelatedMap));
            }

            void OnDetached()
            {
                onDetached();
            }

            void OnMovementStarted(object[] args)
            {
                onMovementStarted((Types.Direction)(args[0]));
            }

            void OnMovementCancelled(object[] args)
            {
                onMovementCancelled((Types.Direction)(args[0]));
            }

            void OnMovementFinished(object[] args)
            {
                onMovementFinished((Types.Direction)(args[0]));
            }

            void OnSolidnessChanged(object[] args)
            {
                onSolidnessChanged((Types.Tilemaps.SolidnessStatus)(args[0]));
            }

            void OnTeleported(object[] args)
            {
                onTeleported((uint)(args[0]), (uint)(args[1]));
            }
        }
    }
}