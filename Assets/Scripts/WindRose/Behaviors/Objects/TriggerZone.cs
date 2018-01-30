using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WindRose
{
    namespace Behaviours
    {
        public abstract class TriggerZone : TriggerHolder
        {
            /**
             * A trigger zone detects entering TriggerActivator elements. It will not do anything but
             *   just receive the events. No implementation will be done of how to react to those events.
             */

            // Registered callbacks. These correspond to the callbacks generated when the
            //   positionable component of a TriggerActivator object changes its position.
            //
            // I will add, perhaps, more triggers later.
            private class MapTriggerCallbacks
            {
                private UnityAction<Positionable> OnMapTriggerPositionChanged;
                private Positionable positionable;
                private uint previousX;
                private uint previousY;

                public MapTriggerCallbacks(Positionable target, UnityAction<Positionable> onMapTriggerPositionChanged, uint x, uint y)
                {
                    OnMapTriggerPositionChanged = onMapTriggerPositionChanged;
                    positionable = target;
                    previousX = x;
                    previousY = y;
                }

                public void CheckPosition()
                {
                    if (positionable.X != previousX || positionable.Y != previousY)
                    {
                        previousX = positionable.X;
                        previousY = positionable.Y;
                        OnMapTriggerPositionChanged(positionable);
                    }
                }
            }
            private Dictionary<TriggerActivator, MapTriggerCallbacks> registeredCallbacks = new Dictionary<TriggerActivator, MapTriggerCallbacks>();

            protected abstract void CallOnMapTriggerEnter(Positionable senderObject);
            protected abstract void CallOnMapTriggerStay(Positionable senderObject);
            protected abstract void CallOnMapTriggerExit(Positionable senderObject);
            protected abstract void CallOnMapTriggerPositionChanged(Positionable senderObject);

            // Register a new sender, and add their callbacks
            void Register(TriggerActivator sender)
            {
                Positionable positionable = sender.GetComponent<Positionable>();
                registeredCallbacks[sender] = new MapTriggerCallbacks(positionable, CallOnMapTriggerPositionChanged, positionable.X, positionable.Y);
            }

            // Gets the registered callbacks, unregisters them.
            void UnRegister(TriggerActivator sender)
            {
                MapTriggerCallbacks cbs = registeredCallbacks[sender];
                registeredCallbacks.Remove(sender);
            }

            void ClearAllCallbacks()
            {
                foreach(KeyValuePair<TriggerActivator, MapTriggerCallbacks> item in registeredCallbacks)
                {
                    ExitAndDisconnect(item.Key);
                }
                registeredCallbacks.Clear();
            }

            void ExitAndDisconnect(TriggerActivator sender)
            {
                CallOnMapTriggerExit(sender.GetComponent<Positionable>());
                UnRegister(sender);
            }

            void ConnectAndEnter(TriggerActivator sender)
            {
                Register(sender);
                Positionable positionable = sender.GetComponent<Positionable>();
                CallOnMapTriggerEnter(positionable);
                // We also should account for other ways of entering the trigger here.
                // One is the trigger has moved, and not the object.
                // The other one is teleporting.
                // Both cases involve the same: The positionable is not performing any movement.
                //   In that case, no movement will be marked later, in the same position.
                // So we trigger that, right now.
                if (positionable.Movement == null)
                {
                    CallOnMapTriggerPositionChanged(positionable);
                }
            }

            void OnDestroy()
            {
                ClearAllCallbacks();
            }

            void OnTriggerEnter2D(Collider2D collision)
            {
                TriggerActivator sender = collision.GetComponent<TriggerActivator>();
                if (sender != null && !registeredCallbacks.ContainsKey(sender))
                {
                    ConnectAndEnter(sender);
                }
            }

            void OnTriggerExit2D(Collider2D collision)
            {
                TriggerActivator sender = collision.GetComponent<TriggerActivator>();
                if (sender != null && registeredCallbacks.ContainsKey(sender))
                {
                    ExitAndDisconnect(sender);
                }
            }

            void Update()
            {
                foreach(KeyValuePair<TriggerActivator, MapTriggerCallbacks> item in registeredCallbacks)
                {
                    CallOnMapTriggerStay(item.Key.GetComponent<Positionable>());
                    item.Value.CheckPosition();
                }
            }
        }
    }
}