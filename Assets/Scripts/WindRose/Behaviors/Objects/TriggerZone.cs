using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Objects
        {
            public abstract class TriggerZone : TriggerHolder
            {
                /**
                 * A trigger zone detects entering TriggerActivator elements. It will not do anything but
                 *   just receive the events. No implementation will be done of how to react to those events.
                 * 
                 * This trigger zone will have somehow a reference to a positionable component. However such
                 *   reference will be determined by the subclasses. Such reference will serve to not relate
                 *   Positionable elements on different maps by mistake.
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

                protected EventDispatcher eventDispatcher;
                protected Positionable positionable;

                // These five events are notified against the involved Positionable components of
                //   already registered TriggerSender objects, the related Positionable object,
                //   and the delta coordinates between them.
                [Serializable]
                public class UnityMapTriggerEvent : UnityEvent<Positionable, Positionable, int, int> { }
                public readonly UnityMapTriggerEvent onMapTriggerEnter = new UnityMapTriggerEvent();
                public readonly UnityMapTriggerEvent onMapTriggerStay = new UnityMapTriggerEvent();
                public readonly UnityMapTriggerEvent onMapTriggerExit = new UnityMapTriggerEvent();
                public readonly UnityMapTriggerEvent onMapTriggerWalked = new UnityMapTriggerEvent();
                public readonly UnityMapTriggerEvent onMapTriggerPlaced = new UnityMapTriggerEvent();
                public readonly UnityMapTriggerEvent onMapTriggerMoved = new UnityMapTriggerEvent();

                protected abstract int GetDeltaX();
                protected abstract int GetDeltaY();

                private void InvokeEventCallback(Positionable senderObject, UnityMapTriggerEvent targetEvent)
                {
                    targetEvent.Invoke(senderObject, positionable, (int)senderObject.X - GetDeltaX(), (int)senderObject.Y - GetDeltaY());
                }

                protected void CallOnMapTriggerEnter(Positionable senderObject)
                {
                    InvokeEventCallback(senderObject, onMapTriggerEnter);
                }

                protected void CallOnMapTriggerStay(Positionable senderObject)
                {
                    InvokeEventCallback(senderObject, onMapTriggerStay);
                }

                protected void CallOnMapTriggerExit(Positionable senderObject)
                {
                    InvokeEventCallback(senderObject, onMapTriggerExit);
                }

                protected void CallOnMapTriggerPlaced(Positionable senderObject)
                {
                    InvokeEventCallback(senderObject, onMapTriggerPlaced);
                    InvokeEventCallback(senderObject, onMapTriggerMoved);
                }

                protected void CallOnMapTriggerWalked(Positionable senderObject)
                {
                    InvokeEventCallback(senderObject, onMapTriggerWalked);
                     InvokeEventCallback(senderObject, onMapTriggerMoved);
                }

                // Register a new sender, and add their callbacks
                void Register(TriggerActivator sender)
                {
                    Positionable positionable = sender.GetComponent<Positionable>();
                    registeredCallbacks[sender] = new MapTriggerCallbacks(positionable, CallOnMapTriggerWalked, positionable.X, positionable.Y);
                }

                // Gets the registered callbacks, unregisters them.
                void UnRegister(TriggerActivator sender)
                {
                    MapTriggerCallbacks cbs = registeredCallbacks[sender];
                    registeredCallbacks.Remove(sender);
                }

                void Withdraw()
                {
                    foreach (KeyValuePair<TriggerActivator, MapTriggerCallbacks> item in registeredCallbacks)
                    {
                        try
                        {
                            ExitAndDisconnect(item.Key);
                        }
                        catch (MissingReferenceException)
                        {
                            // Diaper! No further behaviour needed here
                        }
                    }
                    registeredCallbacks.Clear();
                    collider2D.enabled = false;
                }

                void Appear(Map map)
                {
                    collider2D.enabled = true;
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
                        CallOnMapTriggerPlaced(positionable);
                    }
                }

                void OnDestroy()
                {
                    Withdraw();
                    eventDispatcher.onDetached.RemoveListener(Withdraw);
                    eventDispatcher.onAttached.RemoveListener(Appear);
                }

                protected abstract EventDispatcher GetRelatedEventDispatcher();

                void OnTriggerEnter2D(Collider2D collision)
                {
                    // IF my map is null I will not take any new incoming objects.
                    //   Although this condition will never cause a return in the ideal
                    //   case since when detached the collider will be disabled, this
                    //   condition is the safeguard if the behaviour is somehoe enabled.
                    if (positionable.ParentMap == null) return;

                    // I will only accept TriggerActivator components whose positionables
                    //   are in the same map as this' one.
                    TriggerActivator sender = collision.GetComponent<TriggerActivator>();
                    if (sender == null) return;

                    Positionable senderPositionable = sender.GetComponent<Positionable>();
                    if (positionable.ParentMap != senderPositionable.ParentMap) return;

                    // I will also accept a new entry only if the sender is not already
                    //   registered.
                    if (!registeredCallbacks.ContainsKey(sender))
                    {
                        ConnectAndEnter(sender);
                    }
                }

                void OnTriggerExit2D(Collider2D collision)
                {
                    // Exiting is easier. If I have a registered sender, that sender passed
                    //   all the stated conditions. So I will only check existence and
                    //   registration in order to proceed.
                    TriggerActivator sender = collision.GetComponent<TriggerActivator>();
                    if (sender != null && registeredCallbacks.ContainsKey(sender))
                    {
                        ExitAndDisconnect(sender);
                    }
                }

                protected override void Awake()
                {
                    base.Awake();
                    eventDispatcher = GetRelatedEventDispatcher();
                    positionable = eventDispatcher.GetComponent<Positionable>();
                }

                protected override void Start()
                {
                    base.Start();
                    collider2D.enabled = false;
                    eventDispatcher.onDetached.AddListener(Withdraw);
                    eventDispatcher.onAttached.AddListener(Appear);
                    if (positionable.ParentMap != null) Appear(positionable.ParentMap);
                }

                protected virtual void Update()
                {
                    foreach (KeyValuePair<TriggerActivator, MapTriggerCallbacks> item in registeredCallbacks)
                    {
                        CallOnMapTriggerStay(item.Key.GetComponent<Positionable>());
                        item.Value.CheckPosition();
                    }
                }
            }
        }
    }
}