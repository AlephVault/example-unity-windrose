﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Entities.Objects
        {
            using World;

            /// <summary>
            ///   <para>
            ///     Trigger zones are meant to be... zones inside a map. It detects the presence and
            ///       regular activities of <see cref="TriggerLive"/> objects like walking, moving,
            ///       entering, leaving, staying, and being teleported. Such events will be broadcasted,
            ///       and also the positions will be (normalized and) notified in the event.
            ///   </para>
            /// </summary>
            public abstract class TriggerZone : TriggerHolder
            {
                // Registered callbacks. These correspond to the callbacks generated when the
                //   map object component of a TriggerActivator object changes its position.
                //
                // I will add, perhaps, more triggers later.
                private class MapTriggerCallbacks
                {
                    private UnityAction<Object> OnMapTriggerPositionChanged;
                    private Object mapObject;
                    private uint previousX;
                    private uint previousY;

                    public MapTriggerCallbacks(Object target, UnityAction<Object> onMapTriggerPositionChanged, uint x, uint y)
                    {
                        OnMapTriggerPositionChanged = onMapTriggerPositionChanged;
                        mapObject = target;
                        previousX = x;
                        previousY = y;
                    }

                    public void CheckPosition()
                    {
                        if (mapObject.X != previousX || mapObject.Y != previousY)
                        {
                            previousX = mapObject.X;
                            previousY = mapObject.Y;
                            OnMapTriggerPositionChanged(mapObject);
                        }
                    }
                }
                private Dictionary<TriggerLive, MapTriggerCallbacks> registeredCallbacks = new Dictionary<TriggerLive, MapTriggerCallbacks>();

                /// <summary>
                ///   The fetched related map object. See <see cref="GetRelatedObject"/> for more details.
                /// </summary>
                protected Object mapObject;

                /// <summary>
                ///   A zone event. It will take as argument the triggering object, the map object returned
                ///     in <see cref="GetRelatedObject"/>, and the normalized positions (x, y) as
                ///     returned by subtracting (<see cref="GetDeltaX"/>, <see cref="GetDeltaY"/>).
                /// </summary>
                [Serializable]
                public class UnityMapTriggerEvent : UnityEvent<Object, Object, int, int> { }

                /// <summary>
                ///   Event triggered when an object enters this zone.
                /// </summary>
                public readonly UnityMapTriggerEvent onMapTriggerEnter = new UnityMapTriggerEvent();

                /// <summary>
                ///   Event triggered when an object stays in this zone.
                /// </summary>
                public readonly UnityMapTriggerEvent onMapTriggerStay = new UnityMapTriggerEvent();

                /// <summary>
                ///   Event triggered when an object leaves this zone.
                /// </summary>
                public readonly UnityMapTriggerEvent onMapTriggerExit = new UnityMapTriggerEvent();

                /// <summary>
                ///   Event triggered when an object walked one step in this zone.
                /// </summary>
                public readonly UnityMapTriggerEvent onMapTriggerWalked = new UnityMapTriggerEvent();

                /// <summary>
                ///   Event triggered when an object is teleported in this zone.
                /// </summary>
                public readonly UnityMapTriggerEvent onMapTriggerPlaced = new UnityMapTriggerEvent();

                /// <summary>
                ///   Event triggered when an object walked one step, or is teleported, in this
                ///     zone (triggered after <see cref="onMapTriggerWalked"/> /
                ///     <see cref="onMapTriggerPlaced"/)>.
                /// </summary>
                public readonly UnityMapTriggerEvent onMapTriggerMoved = new UnityMapTriggerEvent();

                /// <summary>
                ///   This method must be implemented to get a delta Y to consider as point of
                ///     reference when calculating the normalized position of the object
                ///     triggering an event in this zone.
                /// </summary>
                /// <returns>The Y to use as delta</returns>
                protected abstract int GetDeltaX();

                /// <summary>
                ///   This method must be implemented to get a delta X to consider as point of
                ///     reference when calculating the normalized position of the object
                ///     triggering an event in this zone.
                /// </summary>
                /// <returns>The X to use as delta</returns>
                protected abstract int GetDeltaY();

                private void InvokeEventCallback(Object senderObject, UnityMapTriggerEvent targetEvent)
                {
                    // When trying to call GetDeltaX() and GetDeltaY() an exception may occur if everything is being destroyed.
                    // That exception occurs because the objects, maps, and strategies are being also destroyed, and now the
                    //   object is not attached to any map (this happens frequently when Destroying/Closing the game).
                    // So the NotAttached exception will be caugh and, if that is the case, then this call with terminate
                    //   immediately. Others exceptions may be NullReferenceException for similar reasons.
                    int x, y;
                    try
                    {
                        x = (int)senderObject.X - GetDeltaX();
                        y = (int)senderObject.Y - GetDeltaY();
                    }
                    catch(Exception) //World.ObjectsManagementStrategies.ObjectsManagementStrategyHolder.NotAttachedException
                    {
                        return;
                    }
                    targetEvent.Invoke(senderObject, mapObject, x, y);
                }

                /// <summary>
                ///   Notifies that an object has entered this zone.
                /// </summary>
                /// <param name="senderObject">The object entering this zone</param>
                protected void CallOnMapTriggerEnter(Object senderObject)
                {
                    InvokeEventCallback(senderObject, onMapTriggerEnter);
                }

                /// <summary>
                ///   Notifies that an object is still in this zone.
                /// </summary>
                /// <param name="senderObject">The object staying in this zone</param>
                protected void CallOnMapTriggerStay(Object senderObject)
                {
                    InvokeEventCallback(senderObject, onMapTriggerStay);
                }

                /// <summary>
                ///   Notifies that an object has left this zone.
                /// </summary>
                /// <param name="senderObject">The object leaving this zone</param>
                protected void CallOnMapTriggerExit(Object senderObject)
                {
                    InvokeEventCallback(senderObject, onMapTriggerExit);
                }

                /// <summary>
                ///   Notifies that an object has been placed in this zone.
                /// </summary>
                /// <param name="senderObject">The object being placed in this zone</param>
                protected void CallOnMapTriggerPlaced(Object senderObject)
                {
                    InvokeEventCallback(senderObject, onMapTriggerPlaced);
                    InvokeEventCallback(senderObject, onMapTriggerMoved);
                }

                /// <summary>
                ///   Notifies that an object walked one step in this zone.
                /// </summary>
                /// <param name="senderObject">The object walking one step in this zone</param>
                protected void CallOnMapTriggerWalked(Object senderObject)
                {
                    InvokeEventCallback(senderObject, onMapTriggerWalked);
                    InvokeEventCallback(senderObject, onMapTriggerMoved);
                }

                // Register a new sender, and add their callbacks
                void Register(TriggerLive sender)
                {
                    Object mapObject = sender.GetComponent<Object>();
                    registeredCallbacks[sender] = new MapTriggerCallbacks(mapObject, CallOnMapTriggerWalked, mapObject.X, mapObject.Y);
                }

                // Gets the registered callbacks, unregisters them.
                void UnRegister(TriggerLive sender)
                {
                    MapTriggerCallbacks cbs = registeredCallbacks[sender];
                    registeredCallbacks.Remove(sender);
                }

                void Withdraw()
                {
                    foreach (TriggerLive key in new List<TriggerLive>(registeredCallbacks.Keys))
                    {
                        try
                        {
                            ExitAndDisconnect(key);
                        }
                        catch (MissingReferenceException)
                        {
                            // Diaper! No further behaviour needed here
                        }
                    }
                    registeredCallbacks.Clear();
                    collider2D.enabled = false;

                    enabled = false;
                }

                void Appear(Map map)
                {
                    collider2D.enabled = true;
                    enabled = true;
                    RefreshDimensions();
                }

                void ExitAndDisconnect(TriggerLive sender)
                {
                    CallOnMapTriggerExit(sender.GetComponent<Object>());
                    UnRegister(sender);
                }

                void ConnectAndEnter(TriggerLive sender)
                {
                    Register(sender);
                    Object mapObject = sender.GetComponent<Object>();
                    CallOnMapTriggerEnter(mapObject);
                    // We also should account for other ways of entering the trigger here.
                    // One is the trigger has moved, and not the object.
                    // The other one is teleporting.
                    // Both cases involve the same: The map object is not performing any movement.
                    //   In that case, no movement will be marked later, in the same position.
                    // So we trigger that, right now.

                    // TODO modificar esta condicion. No vamos a preguntar si el movimiento es null,
                    //   sino tal vez si "aparecio de repente" (aunque le estemos ordenando movimiento,
                    //   pero en tanto ya tenga una posicion adentro del trigger y no se encuentre en la
                    //   fronterita como para gatillar ambos eventos).
                    if (mapObject.Movement == null)
                    {
                        CallOnMapTriggerPlaced(mapObject);
                    }
                }

                void OnDestroy()
                {
                    Withdraw();
                    mapObject.onDetached.RemoveListener(Withdraw);
                    mapObject.onAttached.RemoveListener(Appear);
                }

                /// <summary>
                ///   Gets the related object to this one. This zone will be bound to that map object:
                ///     it will follow it to its map all their life together, and this related map object
                ///     will be the second argument of each event.
                /// </summary>
                /// <returns>The reference map object</returns>
                protected abstract Object GetRelatedObject();

                void OnTriggerEnter2D(Collider2D collision)
                {
                    // IF my map is null I will not take any new incoming objects.
                    //   Although this condition will never cause a return in the ideal
                    //   case since when detached the collider will be disabled, this
                    //   condition is the safeguard if the behaviour is somehoe enabled.
                    if (mapObject.ParentMap == null) return;

                    // I will only accept TriggerActivator components whose map objects
                    //   are in the same map as this' one.
                    TriggerLive sender = collision.GetComponent<TriggerLive>();
                    if (sender == null) return;

                    Object senderMapObject = sender.GetComponent<Object>();
                    if (mapObject.ParentMap != senderMapObject.ParentMap) return;

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
                    TriggerLive sender = collision.GetComponent<TriggerLive>();
                    if (sender != null && registeredCallbacks.ContainsKey(sender))
                    {
                        ExitAndDisconnect(sender);
                    }
                }

                protected override void Awake()
                {
                    base.Awake();
                    mapObject = GetRelatedObject();
                    mapObject.onDetached.AddListener(Withdraw);
                    mapObject.onAttached.AddListener(Appear);
                }

                protected override void Start()
                {
                    base.Start();
                    collider2D.enabled = false;
                    mapObject.onDetached.AddListener(Withdraw);
                    mapObject.onAttached.AddListener(Appear);
                    if (mapObject.ParentMap != null) Appear(mapObject.ParentMap);
                }

                protected virtual void Update()
                {
                    // This change is to avoid OutOfSync error - callbacks MAY and WILL change the inner
                    //   dictionary (of registered callbacks) under some circumstances.
                    foreach (TriggerLive key in new List<TriggerLive>(registeredCallbacks.Keys))
                    {
                        // Perhaps due to data being changed, this condition may evaluate to false!
                        if (registeredCallbacks.ContainsKey(key))
                        {
                            MapTriggerCallbacks value = registeredCallbacks[key];
                            CallOnMapTriggerStay(key.GetComponent<Object>());
                            value.CheckPosition();
                        }
                    }
                }
            }
        }
    }
}