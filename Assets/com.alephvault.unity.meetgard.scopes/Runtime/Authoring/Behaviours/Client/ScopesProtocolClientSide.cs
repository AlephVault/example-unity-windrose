using AlephVault.Unity.Binary;
using AlephVault.Unity.Meetgard.Authoring.Behaviours.Client;
using AlephVault.Unity.Meetgard.Scopes.Types.Constants;
using AlephVault.Unity.Meetgard.Scopes.Types.Protocols;
using AlephVault.Unity.Meetgard.Scopes.Types.Protocols.Messages;
using AlephVault.Unity.Support.Authoring.Behaviours;
using AlephVault.Unity.Support.Generic.Vendor.IUnified.Authoring.Types;
using System;
using System.Collections.Generic;
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
                    /// <summary>
                    ///   A container for a UnityObject implementing the interface:
                    ///   <see cref="IObjectClientSideInstanceManager"/>.
                    /// </summary>
                    [Serializable]
                    public class IObjectClientSideInstanceManagerContainer : IUnifiedContainer<IObjectClientSideInstanceManager> {}

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

                    // The currently loaded objects.
                    private Dictionary<uint, ObjectClientSide> currentObjects = new Dictionary<uint, ObjectClientSide>();

                    // The currently loaded scope id. Specially useful for when the
                    // current scope is a special one.
                    private uint currentScopeId = 0;

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

                    /// <summary>
                    ///   An event for when a local error occurs. Previously, the
                    ///   LocaError message was sent and the connection was closed.
                    /// </summary>
                    public event Action<string> OnLocalError;

                    /// <summary>
                    ///   The local instance manager, if any.
                    /// </summary>
                    [SerializeField]
                    public IObjectClientSideInstanceManagerContainer InstanceManager;

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
                                ClearCurrentScope();
                                currentScopeId = Scope.Limbo;

                                OnWelcome?.Invoke();
                            });
                        });
                        AddIncomingMessageHandler<MovedToScope>("MovedToScope", async (proto, message) => {
                            // The action must be queued in the queueManager.
                            // HOWEVER it will NOT be waited for (the queued
                            // handler will be waited for, but not the returned
                            // handler).
                            var _ = queueManager.QueueTask(async () => {
                                ClearCurrentScope();
                                try
                                {
                                    LoadNewScope(message.ScopeIndex, message.PrefabIndex);
                                    currentScopeId = message.ScopeIndex;
                                }
                                catch(Exception e)
                                {
                                    Debug.LogError($"Exception of type {e.GetType().FullName} while loading a new scope: {e.Message}");
                                    await LocalError("ScopeLoadError");
                                    return;
                                }

                                OnMovedToScope?.Invoke(currentScope);
                            });
                        });
                        AddIncomingMessageHandler<ObjectSpawned>("ObjectSpawned", async (proto, message) => {
                            // The action must be queued in the queueManager.
                            // HOWEVER it will NOT be waited for (the queued
                            // handler will be waited for, but not the returned
                            // handler).
                            var _ = queueManager.QueueTask(async () => {
                                if (currentScope == null || currentScope.Id != message.ScopeIndex || currentScope.Id >= Scope.MaxScopes)
                                {
                                    // This is an error: Either the current scope is null,
                                    // unmatched against the incoming scope index, or the
                                    // incoming scope index being above the maximum amount
                                    // of scopes (e.g. it is Limbo, or Maintenance).
                                    //
                                    // This all will be treated as a local error instead.
                                    Debug.LogError($"Scope mismatch. Current scope is {currentScopeId} and message scope is {message.ScopeIndex}");
                                    await LocalError("ScopeMismatch");
                                    return;
                                }

                                ObjectClientSide spawned;
                                try
                                {
                                    spawned = Spawn(message.ObjectIndex, message.ObjectPrefabIndex, message.Data);
                                }
                                catch(Exception e)
                                {
                                    Debug.LogError($"Exception of type {e.GetType().FullName} while spawning an object: {e.Message}");
                                    await LocalError("SpawnError");
                                    return;
                                }

                                // This event occurs after the per-object spawned event.
                                OnSpawned?.Invoke(spawned);
                            });
                        });
                        AddIncomingMessageHandler<ObjectRefreshed>("ObjectRefreshed", async (proto, message) => {
                            // The action must be queued in the queueManager.
                            // HOWEVER it will NOT be waited for (the queued
                            // handler will be waited for, but not the returned
                            // handler).
                            var _ = queueManager.QueueTask(async () => {
                                if (currentScope == null || currentScope.Id != message.ScopeIndex || currentScope.Id >= Scope.MaxScopes)
                                {
                                    // This is an error: Either the current scope is null,
                                    // unmatched against the incoming scope index, or the
                                    // incoming scope index being above the maximum amount
                                    // of scopes (e.g. it is Limbo, or Maintenance).
                                    //
                                    // This all will be treated as a local error instead.
                                    Debug.LogError($"Scope mismatch. Current scope is {currentScopeId} and message scope is {message.ScopeIndex}");
                                    await LocalError("ScopeMismatch");
                                    return;
                                }

                                Tuple<ObjectClientSide, ISerializable> result;
                                try
                                {
                                    result = Refresh(message.ObjectIndex, message.Data);
                                }
                                catch(Exception e)
                                {
                                    Debug.LogError($"Exception of type {e.GetType().FullName} while refreshing an object: {e.Message}");
                                    await LocalError("RefreshError");
                                    return;
                                }

                                // This event occurs after the per-object refreshed event.
                                OnRefreshed?.Invoke(result.Item1, result.Item2);
                            });
                        });
                        AddIncomingMessageHandler<ObjectDespawned>("ObjectDespawned", async (proto, message) => {
                            // The action must be queued in the queueManager.
                            // HOWEVER it will NOT be waited for (the queued
                            // handler will be waited for, but not the returned
                            // handler).
                            var _ = queueManager.QueueTask(async () => {
                                if (currentScope == null || currentScope.Id != message.ScopeIndex || currentScope.Id >= Scope.MaxScopes)
                                {
                                    // This is an error: Either the current scope is null,
                                    // unmatched against the incoming scope index, or the
                                    // incoming scope index being above the maximum amount
                                    // of scopes (e.g. it is Limbo, or Maintenance).
                                    //
                                    // This all will be treated as a local error instead.
                                    Debug.LogError($"Scope mismatch. Current scope is {currentScopeId} and message scope is {message.ScopeIndex}");
                                    await LocalError("ScopeMismatch");
                                    return;
                                }

                                ObjectClientSide despawned;
                                try
                                {
                                    despawned = Despawn(message.ObjectIndex);
                                }
                                catch(Exception e)
                                {
                                    Debug.LogError($"Exception of type {e.GetType().FullName} while despawning an object: {e.Message}");
                                    await LocalError("DespawnError");
                                    return;
                                }

                                // This event occurs after the per-object despawned event.
                                OnDespawned?.Invoke(despawned);
                            });
                        });
                    }

                    // Clears the current scope, destroying everything.
                    private void ClearCurrentScope()
                    {
                        if (currentScope)
                        {
                            foreach(ObjectClientSide instance in currentObjects.Values)
                            {
                                instance.Despawn();
                                // TODO Allow defining a strategy for spawning (e.g. direct or pooling),
                                // TODO instead of just instantiating the object.
                                Destroy(instance);
                            }
                            currentObjects.Clear();
                            currentScope.Unload();
                            Destroy(currentScope);
                            currentScope = null;
                        }
                    }

                    // Initializes a new scope.
                    private void LoadNewScope(uint scopeId, uint scopePrefabId)
                    {
                        if (scopeId < Scope.MaxScopes)
                        {
                            ScopeClientSide prefab;
                            if (scopePrefabId == Scope.DefaultPrefab)
                            {
                                prefab = defaultScopePrefabs[scopeId - 1];
                            }
                            else
                            {
                                prefab = extraScopePrefabs[scopePrefabId];
                            }
                            ScopeClientSide instance = Instantiate(prefab);
                            instance.Id = scopeId;
                            currentScope = instance;
                            currentScopeId = scopeId;
                            instance.Load();
                        }
                        else
                        {
                            currentScopeId = scopeId;
                        }
                    }

                    // Spawns a new object.
                    private ObjectClientSide Spawn(uint objectId, uint objectPrefabId, byte[] data)
                    {
                        if (currentObjects.ContainsKey(objectId))
                        {
                            throw new InvalidOperationException($"The object id: {objectId} is already in use");
                        }
                        else
                        {
                            // Get a new instance, register it and spawn it.
                            ObjectClientSide instance = InstanceManager.Result != null ? InstanceManager.Result.Get(objectPrefabs[objectPrefabId]) : Instantiate(objectPrefabs[objectPrefabId]);
                            currentObjects.Add(objectId, instance);
                            instance.Spawn(currentScope, objectId, data);
                            return instance;
                        }
                    }

                    // Refreshes an object. Returns both the refreshed object and the
                    // data used for refresh.
                    private Tuple<ObjectClientSide, ISerializable> Refresh(uint objectId, byte[] data)
                    {
                        if (!currentObjects.TryGetValue(objectId, out ObjectClientSide instance))
                        {
                            throw new InvalidOperationException($"The object id: {objectId} is not in use");
                        }
                        else
                        {
                            ISerializable model = instance.Refresh(data);
                            return new Tuple<ObjectClientSide, ISerializable>(instance, model);
                        }
                    }

                    // Despawns an object. Returns the already despawned object.
                    private ObjectClientSide Despawn(uint objectId)
                    {
                        if (!currentObjects.TryGetValue(objectId, out ObjectClientSide instance))
                        {
                            throw new InvalidOperationException($"The object id: {objectId} is not in use");
                        }
                        else
                        {
                            // Despawn the instance, unregister it, and release it.
                            instance.Despawn();
                            currentObjects.Remove(objectId);
                            if (InstanceManager.Result != null) {
                                InstanceManager.Result.Release(instance);
                            } else {
                                Destroy(instance);
                            };
                            // The instance is already unspawned by this point. Depending on the
                            // strategy to use, this may imply the instance is destroyed..
                            return instance;
                        }
                    }

                    /// <summary>
                    ///   Raises a local error and closes the connection.
                    /// </summary>
                    /// <param name="context">
                    ///   The context to raise the error.
                    ///   Only useful locally, for the <see cref="OnLocalError"/> event
                    /// </param>
                    public async Task LocalError(string context)
                    {
                        await SendLocalError();
                        client.Close();
                        OnLocalError?.Invoke(context);
                    }
                }
            }
        }
    }
}