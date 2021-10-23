﻿using AlephVault.Unity.Binary;
using AlephVault.Unity.Meetgard.Authoring.Behaviours.Client;
using AlephVault.Unity.Meetgard.Scopes.Types.Constants;
using AlephVault.Unity.Meetgard.Scopes.Types.Protocols;
using AlephVault.Unity.Meetgard.Scopes.Types.Protocols.Messages;
using AlephVault.Unity.Support.Authoring.Behaviours;
using AlephVault.Unity.Support.Utils;
using System;
using System.Threading.Tasks;
using UnityEngine;


namespace AlephVault.Unity.Meetgard.Scopes
{
    namespace Authoring
    {
        namespace Behaviours
        {
            namespace Client
            {
                /// <summary>
                ///   <para>
                ///     The client side implementation of the scopes-managing protocol.
                ///     It works for a client connection and will be aware of the other
                ///     side (i.e. client) of the scopes the server instantiates over
                ///     the network. It also manages the related objects. For both the
                ///     objects and the scopes, exactly one counterpart will exist in
                ///     the client, and a perfect match must exist to avoid any kind
                ///     of errors and mismatches.
                ///   </para>
                ///   <para>
                ///     Different to the server side, the world is NOT loaded, but
                ///     every time only a single scope is kept as loaded (others will
                ///     be destroyed / somehow unloaded).
                ///   </para>
                /// </summary>
                [RequireComponent(typeof(AsyncQueueManager))]
                public partial class ScopesProtocolClientSide : ProtocolClientSide<ScopesProtocolDefinition>
                {
                    // The queue management dependency.
                    private AsyncQueueManager queueManager;

                    /// <summary>
                    ///   This is the list of scopes that will be paid attention to
                    ///   when receiving a message which instantiates one out of the
                    ///   default scopes (i.e. static scopes).
                    /// </summary>
                    [SerializeField]
                    private ScopeClientSide[] defaultScopePrefabs;

                    /// <summary>
                    ///   This is the list of scopes that will be paid attention to
                    ///   when receiving a message which instantiates one out of the
                    ///   extra scopes (i.e. dynamic scopes).
                    /// </summary>
                    [SerializeField]
                    private ScopeClientSide[] extraScopePrefabs;

                    /// <summary>
                    ///   This is the lis of object prefabs that will be paid attention
                    ///   to when spawning an object.
                    /// </summary>
                    [SerializeField]
                    private ObjectClientSide[] objectPrefabs;

                    // The currently loaded scope.
                    private ScopeClientSide currentScope;

                    // A sender for the LocalError message.
                    private Func<Task> SendLocalError;

                    /// <summary>
                    ///   An event for the Welcome message. When this
                    ///   event is handled, there is no active scope.
                    ///   The only thing that should be done here is:
                    ///   1. Clear any current scope, if any.
                    ///   2. Show some sort of "welcome"/"Loading" page.
                    ///      Each game must do this on its own.
                    /// </summary>
                    public event Action OnWelcome;

                    /// <summary>
                    ///   An event for the MovedToScope message. When
                    ///   this event is handled, there is one active scope.
                    ///   The scope is already loaded, so perhaps additional
                    ///   elements (e.g. hud) may appear on screen. The ids
                    ///   might correspond to the special scopes as well,
                    ///   so those cases must be considered as well when
                    ///   deciding what HUD or stuff to display.
                    /// </summary>
                    public event Action<ScopeClientSide> OnMovedToScope;

                    /// <summary>
                    ///   An event for the ObjectSpawned message. The object
                    ///   is, by this point, already spawned and registered,
                    ///   and their events were already triggered.
                    /// </summary>
                    public event Action<ObjectClientSide> OnSpawned;

                    /// <summary>
                    ///   An event for the ObjectRefreshed message. The object
                    ///   is, by this point, already spawned, registered, and
                    ///   refreshed, and their events were already triggered.
                    /// </summary>
                    public event Action<ObjectClientSide, ISerializable> OnRefreshed;

                    /// <summary>
                    ///   An event for the ObjectDespawned message. The object
                    ///   is, by this point, already despawned andd unregistered,
                    ///   and their events were already triggered.
                    /// </summary>
                    public event Action<ObjectClientSide> OnDespawned;

                    protected override void Initialize()
                    {
                        queueManager = GetComponent<AsyncQueueManager>();
                        SendLocalError = MakeSender("LocalError");
                    }

                    protected override void SetIncomingMessageHandlers()
                    {
                        AddIncomingMessageHandler("Welcome", async (proto) => {
                            // The action must be queued in the queueManager.
                            // HOWEVER it will NOT be waited for (the queued
                            // handler will be waited for, but not the returned
                            // handler).
                            var _ = queueManager.QueueTask(async () => {
                                // TODO: Should this function be async instead?
                                ClearCurrentScope();
                                OnWelcome?.Invoke();
                            });
                        });
                        AddIncomingMessageHandler<MovedToScope>("MovedToScope", async (proto, message) => {
                            // The action must be queued in the queueManager.
                            // HOWEVER it will NOT be waited for (the queued
                            // handler will be waited for, but not the returned
                            // handler).
                            var _ = queueManager.QueueTask(async () => {
                                // TODO: Should this function be async instead?
                                ClearCurrentScope();
                                // TODO: Should this function be async instead?
                                LoadNewScope(message.ScopeIndex, message.PrefabIndex);
                                OnMovedToScope?.Invoke(currentScope);
                            });
                        });
                        AddIncomingMessageHandler<ObjectSpawned>("ObjectSpawned", async (proto, message) => {
                            // On receiving the ObjectSpawned message. The connection
                            // will belong to an actual scope in this case (not Limbo
                            // or a special scope), either default or extra, and in
                            // that scope an object is being spawned. The spawn must
                            // be repeated here, in the client side.
                            //
                            // The action must be queued in the queueManager.
                            // HOWEVER it will NOT be waited for (the queued
                            // handler will be waited for, but not the returned
                            // handler).
                            var _ = queueManager.QueueTask(async () => {
                                if (currentScope == null || currentScope.Id != message.ScopeIndex || currentScope.Id >= Scope.MaxScopes)
                                {
                                    // TODO.
                                    // This is an error: Either the current scope is null,
                                    // unmatched against the incoming scope index, or the
                                    // incoming scope index being above the maximum amount
                                    // of scopes (e.g. it is Limbo, or Maintenance).
                                    //
                                    // This all will be treated as a local error instead.
                                }

                                // TODO: Should this function by async instead?
                                ObjectClientSide spawned = Spawn(message.ObjectIndex, message.ObjectPrefabIndex, message.Data);
                                // This event occurs after the per-object spawned event.
                                OnSpawned?.Invoke(spawned);
                            });
                        });
                        AddIncomingMessageHandler<ObjectRefreshed>("ObjectRefreshed", async (proto, message) => {
                            // On receiving the ObjectRefreshed message. The connection
                            // will belong to an actual scope in this case (not Limbo
                            // or a special scope), either default or extra, and in
                            // that scope an object is being refreshed. The refresh
                            // must be repeated here, in the client side.
                            //
                            // The action must be queued in the queueManager.
                            // HOWEVER it will NOT be waited for (the queued
                            // handler will be waited for, but not the returned
                            // handler).
                            var _ = queueManager.QueueTask(async () => {
                                if (currentScope == null || currentScope.Id != message.ScopeIndex || currentScope.Id >= Scope.MaxScopes)
                                {
                                    // TODO.
                                    // This is an error: Either the current scope is null,
                                    // unmatched against the incoming scope index, or the
                                    // incoming scope index being above the maximum amount
                                    // of scopes (e.g. it is Limbo, or Maintenance).
                                    //
                                    // This all will be treated as a local error instead.
                                }

                                // TODO: Should this function be async instead?
                                Tuple<ObjectClientSide, ISerializable> result = Refresh(message.ObjectIndex, message.Data);
                                // This event occurs after the per-object refreshed event.
                                OnRefreshed?.Invoke(result.Item1, result.Item2);
                            });
                        });
                        AddIncomingMessageHandler<ObjectDespawned>("ObjectDespawned", async (proto, message) => {
                            // On receving the ObjectDespawned message. The connection
                            // will belong to an actual scope in this case (not Limbo
                            // or a special scope), either default or extra, and in
                            // that scope an object is being despawned. The despawn
                            // must be repeated here, in the client side.
                            //
                            // The action must be queued in the queueManager.
                            // HOWEVER it will NOT be waited for (the queued
                            // handler will be waited for, but not the returned
                            // handler).
                            var _ = queueManager.QueueTask(async () => {
                                if (currentScope == null || currentScope.Id != message.ScopeIndex || currentScope.Id >= Scope.MaxScopes)
                                {
                                    // TODO.
                                    // This is an error: Either the current scope is null,
                                    // unmatched against the incoming scope index, or the
                                    // incoming scope index being above the maximum amount
                                    // of scopes (e.g. it is Limbo, or Maintenance).
                                    //
                                    // This all will be treated as a local error instead.
                                }

                                // TODO: Should this function be async instead?
                                ObjectClientSide despawned = Despawn(message.ObjectIndex);
                                // This event occurs after the per-object despawned event.
                                OnDespawned?.Invoke(despawned);
                            });
                        });
                    }

                    // Clears the current scope, destroying everything.
                    private void ClearCurrentScope()
                    {
                        // TODO: Should this function be async instead?
                        // TODO implement.
                    }

                    // Initializes a new scope.
                    private void LoadNewScope(uint scopeId, uint scopePrefabId)
                    {
                        // TODO: Should this function be async instead?
                        // TODO implement.
                    }

                    // Spawns a new object.
                    private ObjectClientSide Spawn(uint objectId, uint objectPrefabId, byte[] data)
                    {
                        // TODO: Should this function be async instead?
                        // TODO implement (instantiate, spawn, register, trigger event).
                        // TODO allow defining a strategy for spawning (e.g. direct or pooling).
                        return null;
                    }

                    // Refreshes an object. Returns both the refreshed object and the
                    // data used for refresh.
                    private Tuple<ObjectClientSide, ISerializable> Refresh(uint objectId, byte[] data)
                    {
                        // TODO: Should this function be async instead?
                        // TODO implement (check, refresh, trigger event, return the object).
                        return null;
                    }

                    // Despawns an object. Returns the already despawned object.
                    private ObjectClientSide Despawn(uint objectId)
                    {
                        // TODO: Should this function be async instead?
                        // TODO implement (check, despawn, trigger event, return the object).
                        // TODO allow defining a strategy for despawning (e.g. direct or pooling).
                        return null;
                    }
                }
            }
        }
    }
}