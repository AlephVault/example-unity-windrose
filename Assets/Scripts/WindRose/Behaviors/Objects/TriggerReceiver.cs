using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WindRose
{
    namespace Behaviours
    {
        [RequireComponent(typeof(TriggerEnabled))]
        public class TriggerReceiver : MonoBehaviour
        {
            /**
             * A TriggerReceiver will also correctly compute its collision mask.
             * 
             * However its purpose will be different (and complementary) to
             *   TriggerSender.
             * 
             * This component will (TODO) the following behaviour on each TriggerSender:
             *   1. A trigger sender has just fulfilled two conditions:
             *      i. be in the same map as this trigger receiver.
             *      ii. staying inside this trigger receiver.
             *   2. A trigger sender has just fulfilled one or more of these conditions:
             *      i. be in different map.
             *      ii. getting out of this trigger receivr.
             *   3. A trigger sender is inside this map, and this trigger receiver.
             *   4. OnDestroy will clear all event callbacks on all currently staying TriggerSenders.
             *   5. The installed callback will attend the "it moved!" event.
             *   6. The installed callback will attend the "it teleported!" event.
             *   
             *   All these (5) events have to be attended by new (forwarded) events of this TriggerReceiver.
             *   Other behaviours that could depend on TriggerReceiver may (and will) add callbacks to the
             *     new events TriggerReceiver provides.
             */
                        
            private Positionable positionable;

            // Registered callbacks. These correspond to the callbacks actually registered in the
            //   EventDispatcher component of a TriggerSender object.
            //
            // I will add, perhaps, more triggers later.
            private class MapTriggerCallbacks
            {
                public readonly UnityAction<Types.Direction> OnMapTriggerMoved;

                public MapTriggerCallbacks(UnityAction<Types.Direction> onMapTriggerMoved)
                {
                    OnMapTriggerMoved = onMapTriggerMoved;
                }
            }
            private Dictionary<TriggerSender, MapTriggerCallbacks> registeredCallbacks = new Dictionary<TriggerSender, MapTriggerCallbacks>();

            // These five events are notified against the involved Positionable components of
            //   already registered TriggerSender objects, the positionable of this object,
            //   and the delta coordinates between them.
            [Serializable]
            public class UnityMapTriggerEvent : UnityEvent<Positionable, Positionable, int, int> {}
            public readonly UnityMapTriggerEvent onMapTriggerEnter = new UnityMapTriggerEvent();
            public readonly UnityMapTriggerEvent onMapTriggerStay = new UnityMapTriggerEvent();
            public readonly UnityMapTriggerEvent onMapTriggerExit = new UnityMapTriggerEvent();
            public readonly UnityMapTriggerEvent onMapTriggerMoved = new UnityMapTriggerEvent();

            private void InvokeEventCallback(Positionable senderObject, UnityMapTriggerEvent targetEvent)
            {
                targetEvent.Invoke(senderObject, positionable, (int)senderObject.X - (int)positionable.X, (int)senderObject.Y - (int)positionable.Y);
            }

            private void CallOnMapTriggerEnter(Positionable senderObject)
            {
                InvokeEventCallback(senderObject, onMapTriggerEnter);
            }

            private void CallOnMapTriggerStay(Positionable senderObject)
            {
                InvokeEventCallback(senderObject, onMapTriggerStay);
            }

            private void CallOnMapTriggerExit(Positionable senderObject)
            {
                InvokeEventCallback(senderObject, onMapTriggerExit);
            }

            private void CallOnMapTriggerMoved(Positionable senderObject)
            {
                InvokeEventCallback(senderObject, onMapTriggerMoved);
            }

            // Register a new sender, and add their callbacks
            void Register(TriggerSender sender)
            {
                registeredCallbacks[sender] = new MapTriggerCallbacks((direction) => {
                    CallOnMapTriggerMoved(sender.GetComponent<Positionable>());
                });

                sender.GetComponent<EventDispatcher>().onMovementFinished.AddListener(registeredCallbacks[sender].OnMapTriggerMoved);
            }

            // Gets the registered callbacks, unregisters them, and removes them from the related object.
            void UnRegister(TriggerSender sender)
            {
                MapTriggerCallbacks cbs = registeredCallbacks[sender];
                registeredCallbacks.Remove(sender);
                sender.GetComponent<EventDispatcher>().onMovementFinished.AddListener(cbs.OnMapTriggerMoved);
            }

            void ClearAllCallbacks()
            {
                foreach(KeyValuePair<TriggerSender, MapTriggerCallbacks> item in registeredCallbacks)
                {
                    ExitAndDisconnect(item.Key);
                }
                registeredCallbacks.Clear();
            }

            void ExitAndDisconnect(TriggerSender sender)
            {
                CallOnMapTriggerExit(sender.GetComponent<Positionable>());
                UnRegister(sender);
            }

            void ConnectAndEnter(TriggerSender sender)
            {
                Register(sender);
                CallOnMapTriggerEnter(sender.GetComponent<Positionable>());
            }

            void OnDestroy()
            {
                ClearAllCallbacks();
            }

            void OnTriggerEnter2D(Collider2D collision)
            {
                TriggerSender sender = collision.GetComponent<TriggerSender>();
                if (sender != null && !registeredCallbacks.ContainsKey(sender))
                {
                    ConnectAndEnter(sender);
                }
            }

            void OnTriggerExit2D(Collider2D collision)
            {
                TriggerSender sender = collision.GetComponent<TriggerSender>();
                if (sender != null && registeredCallbacks.ContainsKey(sender))
                {
                    ExitAndDisconnect(sender);
                }
            }

            void Update()
            {
                foreach (KeyValuePair<TriggerSender, MapTriggerCallbacks> item in registeredCallbacks)
                {
                    CallOnMapTriggerStay(item.Key.GetComponent<Positionable>());
                }
            }

            void Start()
            {
                positionable = GetComponent<Positionable>();
                if (positionable.Solidness != Types.Tilemaps.SolidnessStatus.Ghost && positionable.Solidness != Types.Tilemaps.SolidnessStatus.Hole)
                {
                    positionable.SetSolidness(Types.Tilemaps.SolidnessStatus.Ghost);
                }
            }
        }
    }
}