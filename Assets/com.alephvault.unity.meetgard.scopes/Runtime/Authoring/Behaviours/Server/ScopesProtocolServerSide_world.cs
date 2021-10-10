using AlephVault.Unity.Meetgard.Authoring.Behaviours.Server;
using AlephVault.Unity.Meetgard.Scopes.Types.Constants;
using AlephVault.Unity.Meetgard.Scopes.Types.Protocols;
using AlephVault.Unity.Support.Types;
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
            namespace Server
            {
                public partial class ScopesProtocolServerSide : ProtocolServerSide<ScopesProtocolDefinition>
                {
                    /// <summary>
                    ///   The world load status. This only stands for the default
                    ///   scopes (dynamic scopes are not included here, and any
                    ///   potential load error or other exception will be handled
                    ///   appropriately - without destroying the whole server).
                    /// </summary>
                    public LoadStatus WorldLoadStatus { get; private set; }

                    // The currently loaded scope server sides, mapped against their
                    // assigned id.
                    private Dictionary<uint, ScopeServerSide> loadedScopes = new Dictionary<uint, ScopeServerSide>();

                    // The ids generator for the currently loaded scope server sides.
                    private IdPool loadedScopesIds = new IdPool(Scope.MaxScopes);

                    /// <summary>
                    ///   This event is triggered when an error occurs while trying
                    ///   to load the whole world. The intention of this handler
                    ///   is to log the exception somewhere. It is advised to check
                    ///   any exception that may be raised here, since they will be
                    ///   otherwise captured and hidden.
                    /// </summary>
                    public event Action<System.Exception> OnLoadError = null;

                    /// <summary>
                    ///   This event is triggered when an error occurs while trying
                    ///   to unload a particular world scene. The intention of this
                    ///   handler is to log the exception somewhere. It is advised
                    ///   to check any exception that may be raised here, since they
                    ///   will be otherwise captured and hidden.
                    /// </summary>
                    public event Action<uint, ScopeServerSide, System.Exception> OnUnloadError = null;

                    // Loads all of the default scopes. For each attempted entry,
                    // one instance will be added to the loaded scopes. Such instance
                    // will not just be instantiated but also initialized (e.g. its
                    // data being loaded from database). Any exception raised here
                    // will cause all the scopes to be unloaded and the server to be
                    // closed, if already open.
                    private async Task LoadDefaultScopes()
                    {
                        loadedScopes = new Dictionary<uint, ScopeServerSide>();
                        loadedScopesIds = new IdPool(Scope.MaxScopes);
                        uint index = 0;
                        foreach(ScopeServerSide scopePrefab in defaultScopePrefabs)
                        {
                            ScopeServerSide instance = Instantiate(scopePrefab, null, true);
                            await instance.Load();
                            uint newId = (uint)loadedScopesIds.Next();
                            instance.ID = newId;
                            instance.PrefabID = Scope.DefaultPrefab;
                            instance.Protocol = this;
                            loadedScopes.Add(newId, instance);
                            index++;
                        }
                    }

                    // Unloads all of the loaded scopes, one by one. This may involve
                    // things like storing the last scope state back into database and
                    // that sort of things, in a per-scope basis. Any exception that
                    // occurs in this process will be handled in a per-scope basis.
                    private async Task UnloadScopes()
                    {
                        foreach(KeyValuePair<uint, ScopeServerSide> pair in loadedScopes)
                        {
                            if (pair.Value != null)
                            {
                                try
                                {
                                    await ClearConnectionsFromScope(pair.Value);
                                    await pair.Value.Unload();
                                }
                                catch (System.Exception e)
                                {
                                    try
                                    {
                                        OnUnloadError?.Invoke(pair.Key, pair.Value, e);
                                    }
                                    catch(System.Exception)
                                    {
                                        Debug.LogError(
                                            "An exception has been triggered while handling a previous exception " +
                                            "in OnUnloadError while trying to unload the world. This should not " +
                                            "occur. Ensure any exception that may occur inside an OnUnloadError " +
                                            "handler is properly handled inside that handler, instead of letting " +
                                            "it bubble"
                                        );
                                    }
                                }
                            }
                        }
                    }

                    // Clears the collection of loaded scopes, and destroys
                    // the scopes one by one.
                    private void DestroyInstantiatedScopes()
                    {
                        Dictionary<uint, ScopeServerSide> instances = loadedScopes;
                        loadedScopes = null;
                        foreach(KeyValuePair<uint, ScopeServerSide> pair in instances)
                        {
                            if (pair.Value != null) Destroy(pair.Value);
                            pair.Value.ID = 0;
                            pair.Value.Protocol = null;
                            loadedScopesIds.Release(pair.Key);
                        }
                    }

                    // Queues the whole world loading function. This task will
                    // try to load the world and ensure it becomes "ready". If
                    // any error occurs on world load, then
                    private Task LoadWorld()
                    {
                        return queueManager.QueueTask(async () => {
                            // This makes no sense when the world is not unloaded.
                            if (WorldLoadStatus != LoadStatus.Empty) return;

                            // Set the initial, in-progress, status.
                            WorldLoadStatus = LoadStatus.Loading;

                            try
                            {
                                // Do the whole load.
                                await LoadDefaultScopes();

                                // Set the final, success, status.
                                WorldLoadStatus = LoadStatus.Ready;
                            }
                            catch (System.Exception e)
                            {
                                // Set a temporary error status.
                                WorldLoadStatus = LoadStatus.LoadError;

                                // Diaper-log any load exception.
                                try
                                {
                                    OnLoadError?.Invoke(e);
                                }
                                catch
                                {
                                    Debug.LogError(
                                        "An exception has been triggered while handling a previous exception " +
                                        "in OnLoadError while trying to load the world. This should not occur. " +
                                        "Ensure any exception that may occur inside an OnLoadError handler is " +
                                        "properly handled inside that handler, instead of letting it bubble"
                                    );
                                }

                                // Destroy the scopes. There is no Unload
                                // needed, since no change occurred in the
                                // scope per se (or: there is no sense for
                                // any change to be accounted for, since
                                // the game hasn't yet started).
                                DestroyInstantiatedScopes();

                                // Set the final, reset, status.
                                WorldLoadStatus = LoadStatus.Empty;

                                // And finally, close the server.
                                if (server.IsListening) server.StopServer();
                            }
                        });
                    }

                    // Queues the whole world unloading function. This task will
                    // try to unload the world and ensure it becomes "empty". The
                    // unload errors will be each caught separately and logged
                    // separately as well, but the process will ultimately finish
                    // and the world will become "empty". It is recommended that
                    // no unload brings any error, since unloading will also mean
                    // data backup / store and related stuff. By the end, the world
                    // will become empty, and both default and extra scopes will
                    // be both unloaded and destroyed.
                    private Task UnloadWorld()
                    {
                        return queueManager.QueueTask(async () =>
                        {
                            // This makes no sense when the world is not loaded.
                            if (WorldLoadStatus != LoadStatus.Ready) return;

                            // Set the initial, in-progress, status.
                            WorldLoadStatus = LoadStatus.Unloading;

                            // Unload all of the scopes. Any exception
                            // will be handled and/or diapered separately.
                            await UnloadScopes();

                            // Destroy the already unloaded scopes. In the
                            // worst case, the lack of backup will restore
                            // the scope state to a previous state, with
                            // some elements not so fully synchronized and,
                            // if well managed by the per-game logic, that
                            // will not affect the overall game experience.
                            DestroyInstantiatedScopes();

                            // Set the final, success, status.
                            WorldLoadStatus = LoadStatus.Empty;
                        });
                    }

                    /// <summary>
                    ///   Instantiates a scope by specifying its prefab key and initializing it.
                    ///   The scope will be registered, assigned an ID, and returned. This task
                    ///   is queued.
                    /// </summary>
                    /// <param name="extraScopePrefabKey">
                    ///   The key of an extra scope prefab to use. As a tip, as the extra
                    ///   scope prefabs are configurable, let the value for this argument
                    ///   come from a configurable source (i.e. editor, authoring) and not
                    ///   a constant or hard-coded value in the codebase
                    /// </param>
                    /// <param name="init">A function to initialize the scope to be loaded</param>
                    /// <returns>The loaded (and registered) scope instance</returns>
                    public Task<ScopeServerSide> LoadExtraScope(string extraScopePrefabKey, Action<ScopeServerSide> init)
                    {
                        return queueManager.QueueTask<ScopeServerSide>(async () =>
                        {
                            if (WorldLoadStatus != LoadStatus.Ready)
                            {
                                throw new InvalidOperationException(
                                    "The world is currently not ready to load an extra scope"
                                );
                            }

                            uint extraScopePrefabIndex;
                            try
                            {
                                extraScopePrefabIndex = extraScopePrefabIndicesByKey[extraScopePrefabKey];
                            }
                            catch (KeyNotFoundException)
                            {
                                throw new ArgumentException($"Unknown extra scope prefab key: {extraScopePrefabKey}");
                            }

                            ScopeServerSide instance = Instantiate(extraScopePrefabs[extraScopePrefabIndex], null, true);
                            await instance.Load();
                            uint newId = (uint)loadedScopesIds.Next();
                            instance.ID = newId;
                            instance.PrefabID = Scope.DefaultPrefab;
                            instance.Protocol = this;
                            loadedScopes.Add(newId, instance);
                            return instance;
                        });
                    }

                    // Unloads and perhaps destroys a scope.
                    private async Task DoUnloadExtraScope(uint scopeID, ScopeServerSide scopeToUnload, bool destroy)
                    {
                        await ClearConnectionsFromScope(scopeToUnload);
                        await scopeToUnload.Unload();
                        if (scopeToUnload != null && destroy) Destroy(scopeToUnload);
                        scopeToUnload.ID = 0;
                        scopeToUnload.PrefabID = 0;
                        scopeToUnload.Protocol = null;
                        loadedScopesIds.Release(scopeID);
                    }

                    /// <summary>
                    ///   Unloads and perhaps destroys a scope. This task is queued.
                    /// </summary>
                    /// <param name="scopeID">The id of the scope to unload</param>
                    /// <param name="destroy">Whether to also destroy it or not</param>
                    public Task UnloadExtraScope(uint scopeID, bool destroy = true)
                    {
                        return queueManager.QueueTask(async () => {
                            if (scopeID <= defaultScopePrefabs.Length)
                            {
                                throw new ArgumentException(
                                    $"Cannot delete the scope with ID: {scopeID} since that ID belongs " +
                                    $"to the set of default scopes"
                                );
                            }

                            ScopeServerSide scopeToUnload;
                            try
                            {
                                scopeToUnload = loadedScopes[scopeID];
                            }
                            catch (KeyNotFoundException)
                            {
                                throw new ArgumentException(
                                    $"Cannot delete the scope with ID: {scopeID} since that ID belongs " +
                                    $"to the set of default scopes"
                                );
                            }

                            await DoUnloadExtraScope(scopeID, scopeToUnload, destroy);
                        });
                    }

                    /// <summary>
                    ///   Unloads and perhaps destroys a scope. This task is queued.
                    /// </summary>
                    /// <param name="scope">The scope instance to unload</param>
                    /// <param name="destroy">Whether to also destroy it or not</param>
                    public Task UnloadExtraScope(ScopeServerSide scope, bool destroy = true)
                    {
                        return queueManager.QueueTask(async () => {
                            if (scope == null)
                            {
                                throw new ArgumentNullException("scope");
                            }
                            else if (scope.Protocol != this)
                            {
                                throw new ArgumentException("The given scope does not belong to this server - it cannot be deleted");
                            }

                            uint scopeID = scope.ID;
                            if (scopeID <= defaultScopePrefabs.Length)
                            {
                                throw new ArgumentException(
                                    $"Cannot delete the scope, which has ID: {scopeID} since that scope " +
                                    $"belongs to the set of default scopes"
                                );
                            }

                            await DoUnloadExtraScope(scopeID, scope, destroy);
                        });
                    }
                }
            }
        }
    }
}