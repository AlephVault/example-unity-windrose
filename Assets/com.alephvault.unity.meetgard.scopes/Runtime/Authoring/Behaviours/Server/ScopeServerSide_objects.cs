using AlephVault.Unity.Binary;
using AlephVault.Unity.Meetgard.Scopes.Types.Constants;
using AlephVault.Unity.Meetgard.Scopes.Types.Protocols.Messages;
using AlephVault.Unity.Support.Types;
using AlephVault.Unity.Support.Utils;
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
                public partial class ScopeServerSide : MonoBehaviour
                {
                    // This is the list of objects registered in this
                    // scope. The id given to them is scope-specific.
                    private Dictionary<uint, ObjectServerSide> objects = new Dictionary<uint, ObjectServerSide>();

                    // This is the list of IDs registered in this scope.
                    private IdPool objectIds = new IdPool(uint.MaxValue);

                    // This is the array to send custom object data.
                    private byte[] fullObjectData;

                    /// <summary>
                    ///   Triggered when an object is spawned inside the
                    ///   current scope.
                    /// </summary>
                    public event Func<ObjectServerSide, Task> OnSpawned = null;

                    /// <summary>
                    ///   Triggered when an object is despawned from the
                    ///   last scope. By this point, the object will not
                    ///   have any sort of scope association information.
                    /// </summary>
                    public event Func<ObjectServerSide, Task> OnDespawned = null;

                    // Registers a new object in this scope.
                    private void RegisterObject(ObjectServerSide target)
                    {
                        uint newId = (uint)objectIds.Next();
                        target.Id = newId;
                        target.Scope = this;
                        objects.Add(newId, target);
                    }

                    // Unregisters an object from this scope.
                    private void UnregisterObject(ObjectServerSide target)
                    {
                        objectIds.Release(target.Id);
                        objects.Remove(target.Id);
                        target.Id = 0;
                        target.Scope = null;
                    }

                    // Broadcasts the object's data to all of the available
                    // connections in the scope.
                    private void NotifyObjectSpawnedToEveryone(ObjectServerSide target)
                    {
                        XDebug debugger = new XDebug("Meetgard.Scopes", this, $"NotifyObjectSpawnedToEveryone({Id}, {target.Id})", debug);
                        debugger.Start();
                        foreach(Tuple<HashSet<ulong>, ISerializable> pair in target.FullData(connections))
                        {
                            if (pair.Item2 == null)
                            {
                                Debug.LogWarning("Trying to spawn using a null value as data - the spawn was ignored");
                                continue;
                            }

                            // Lazy-allocate the array.
                            if (fullObjectData == null) fullObjectData = Protocol.AllocateFullDataMessageBytes();
                            // Serialize the object into the array.
                            pair.Item2.Serialize(new Serializer(new Writer(new Binary.Buffer(fullObjectData))));                            
                            // Notify everyone.
                            Protocol.BroadcastObjectSpawned(pair.Item1, new ObjectSpawned() {
                                ObjectIndex = target.Id,
                                ObjectPrefabIndex = target.PrefabId,
                                ScopeIndex = Id,
                                Data = fullObjectData
                            });
                        }
                        debugger.End();
                    }

                    // Broadcasts the object disappearance from the scope.
                    private void NotifyObjectDespawnedToEveryone(ObjectServerSide target)
                    {
                        XDebug debugger = new XDebug("Meetgard.Scopes", this, $"NotifyObjectDespawnedToEveryone({Id}, {target.Id})", debug);
                        debugger.Start();
                        Protocol.BroadcastObjectDespawned(connections, new ObjectDespawned() {
                            ObjectIndex = target.Id,
                            ScopeIndex = Id
                        });
                        debugger.End();
                    }

                    /// <summary>
                    ///   <para>
                    ///     Adds an object to the current scope. If the object
                    ///     is already added to a scope, this is an error. If
                    ///     the object belongs to a different server this scope
                    ///     belongs to, it is an error. This task is queued in
                    ///     the current queue.
                    ///   </para>
                    ///   <para>
                    ///     This method can be called standalone or will be called
                    ///     from the object itself, on hierarchy change.
                    ///   </para>
                    /// </summary>
                    /// <param name="obj">The object to add</param>
                    public Task AddObject(ObjectServerSide target)
                    {
                        return Protocol.RunInMainThread(async () =>
                        {
                            XDebug debugger = new XDebug("Meetgard.Scopes", this, $"AddObject(scope {Id})", debug);
                            debugger.Start();
                            debugger.Info("Checking parameters and status");

                            // Null/Destroyed objects cannot be added.
                            if (target == null) throw new ArgumentNullException("target");

                            // Objects in a different protocol cannot be added.
                            if (target.Protocol != Protocol) throw new ArgumentException("The target to add was not created with the same protocol instance");

                            // Ignore if the scope is the same - the object is already
                            // added to this scope.
                            if (target.Scope == this) return;

                            // Objects belonging to other scopes cannot be added.
                            if (target.Scope != null) throw new ArgumentException("The target to add already belongs to a scope");

                            // Then we force the object to be descendant of this scope.
                            debugger.Info("Setting the parent");
                            if (target.GetComponentInParent<ScopeServerSide>() != this)
                            {
                                target.transform.SetParent(transform);
                            }

                            // Finally, register the object, and broadcast.
                            debugger.Info("Registering the object");
                            RegisterObject(target);
                            debugger.Info("Notifying the object being spawned");
                            NotifyObjectSpawnedToEveryone(target);

                            // In the end, trigger the event for the object.
                            debugger.Info("Notifying locally (to the object)");
                            await (target.TriggerOnSpawned() ?? Task.CompletedTask);
                            debugger.Info("Notifying locally (to the scope)");
                            await (OnSpawned?.InvokeAsync(target, async (e) => {
                                Debug.LogError(
                                    $"An error of type {e.GetType().FullName} has occurred in scope server side's OnSpawned event. " +
                                    $"If the exceptions are not properly handled, the game state might be inconsistent. " +
                                    $"The exception details are: {e.Message}"
                                );
                            }) ?? Task.CompletedTask);
                            debugger.End();
                        });
                    }

                    /// <summary>
                    ///   <para>
                    ///     Removes an object from the current scope. If the
                    ///     object is not added to this scope, this is an error.
                    ///     This task is queued in the current queue.
                    ///   </para>
                    ///   <para>
                    ///     This method can be called standalone or will be called
                    ///     from the object itself, on hierarchy change.
                    ///   </para>
                    /// </summary>
                    /// <param name="obj">The object to remove</param>
                    public Task RemoveObject(ObjectServerSide target)
                    {
                        return Protocol.RunInMainThread(async () =>
                        {
                            XDebug debugger = new XDebug("Meetgard.Scopes", this, $"RemoveObject(scope {Id}, {target.Id})", debug);
                            debugger.Start();
                            debugger.Info("Checking parameters and status");

                            // Null objects cannot be removed.
                            if (ReferenceEquals(target, null)) throw new ArgumentNullException("target");

                            // Objects not belonging to any scope will end silently.
                            if (target.Scope == null) return;

                            // Objects not belonging to this scope cannot be removed.
                            if (target.Scope != this) throw new ArgumentException("The target to remove does not belong to this scope");

                            // Then we force the object to be not descendant of this object.
                            // We do this while the object is not destroyed. Otherwise, we
                            // skip this part.
                            debugger.Info("Setting the parent");
                            if (target != null && target.GetComponentInParent<ScopeServerSide>() == this)
                            {
                                target.transform.SetParent(transform.parent);
                            }

                            // Finally, unregister the object, and broadcast.
                            debugger.Info("Notifying the object being despawned");
                            NotifyObjectDespawnedToEveryone(target);
                            debugger.Info("Unregistering the object");
                            UnregisterObject(target);

                            // In the end, trigger the event for the object.
                            debugger.Info("Notifying locally (to the object)");
                            await (target.TriggerOnDespawned() ?? Task.CompletedTask);
                            debugger.Info("Notifying locally (to the scope)");
                            await (OnDespawned?.InvokeAsync(target, async (e) => {
                                Debug.LogError(
                                    $"An error of type {e.GetType().FullName} has occurred in scope server side's OnDespawned event. " +
                                    $"If the exceptions are not properly handled, the game state might be inconsistent. " +
                                    $"The exception details are: {e.Message}"
                                );
                            }) ?? Task.CompletedTask);
                            debugger.End();
                        });
                    }

                    /// <summary>
                    ///   <para>
                    ///     For a given connection, it refreshes all of its objects,
                    ///     considering a given refresh context. Typically, this
                    ///     method should be needed few times... not per connection
                    ///     but per underlying profile. This operation is heave and,
                    ///     like the initial sync, it may consume some degree of
                    ///     network resources.
                    ///   </para>
                    /// </summary>
                    /// <param name="connection">The connection that needs a refresh</param>
                    /// <param name="context">The refresh context</param>
                    public Task RefreshExistingObjectsTo(ulong connection, string context)
                    {
                        return Protocol.RunInMainThread(async () =>
                        {
                            XDebug debugger = new XDebug("Meetgard.Scopes", this, $"RefreshExistingObjectsTo({connection}, {context})", debug);
                            debugger.Start();
                            foreach (ObjectServerSide obj in objects.Values)
                            {
                                ISerializable refreshData = obj.RefreshData(connection, context);
                                if (refreshData != null)
                                {
                                    // Lazy-allocate the array.
                                    if (fullObjectData == null) fullObjectData = Protocol.AllocateFullDataMessageBytes();
                                    // Serialize the object into the array.
                                    refreshData.Serialize(new Serializer(new Writer(new Binary.Buffer(fullObjectData))));
                                    Protocol.SendObjectRefreshed(connection, new ObjectRefreshed() {
                                        ObjectIndex = obj.Id,
                                        ScopeIndex = Id,
                                        Data = fullObjectData
                                    });
                                }
                            }
                            debugger.End();
                        });
                    }

                    /// <summary>
                    ///   Returns an iterator of all the objects in the scope.
                    /// </summary>
                    /// <returns>The iterator</returns>
                    public IEnumerable<ObjectServerSide> Objects()
                    {
                        return objects.Values;
                    }

                    // Synchronizes all the existing objects to the connection.
                    // This synchronization is initial, and will be part of a
                    // bigger queued task.
                    private async Task SyncExistingObjectsTo(ulong connection)
                    {
                        XDebug debugger = new XDebug("Meetgard.Scopes", this, $"SyncExistingObjectsTo({connection})", debug);
                        debugger.Start();
                        foreach(ObjectServerSide obj in objects.Values)
                        {
                            // Lazy-allocate the array.
                            if (fullObjectData == null) fullObjectData = Protocol.AllocateFullDataMessageBytes();
                            // Serialize the object into the array.
                            obj.FullData(connection).Serialize(new Serializer(new Writer(new Binary.Buffer(fullObjectData))));
                            Protocol.SendObjectSpawned(connection, new ObjectSpawned()
                            {
                                ObjectIndex = obj.Id,
                                ObjectPrefabIndex = obj.PrefabId,
                                ScopeIndex = Id,
                                Data = fullObjectData
                            });
                        }
                        debugger.End();
                    }
                }
            }
        }
    }
}
