using AlephVault.Unity.Binary;
using AlephVault.Unity.Meetgard.Authoring.Behaviours.Server;
using GameMeanMachine.Unity.NetRose.Authoring.Behaviours.Models.Entities.Objects;
using GameMeanMachine.Unity.NetRose.Types.Protocols.Messages;
using GameMeanMachine.Unity.NetRose.Types.Protocols;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameMeanMachine.Unity.WindRose.Types;
using AlephVault.Unity.Binary.Wrappers;

namespace GameMeanMachine.Unity.NetRose
{
    namespace Authoring
    {
        namespace Protocols
        {
            /// <summary>
            ///   <para>
            ///     A NetRose server-side implementation involves a list of
            ///     registered model server-side classes (primary and watched)
            ///     and this is related to the models registered in a particular
            ///     set of registered models in the underlying protocol definition.
            ///   </para>
            ///   <para>
            ///     It is related to a particular NetRose protocol definition.
            ///   </para>
            /// </summary>
            /// <typeparam name="Definition">The NetRose protocol definition this</typeparam>
            public abstract class NetRoseProtocolServerSide<Definition> : ProtocolServerSide<Definition>, INetRoseProtocolServerSideSender
                where Definition : NetRoseProtocolDefinition, new()
            {
                /// <summary>
                ///   This model does not define, at least so far, any kind of
                ///   incoming messages. Game logic should be done in its own
                ///   (parallel) protocol.
                /// </summary>
                protected override void SetIncomingMessageHandlers()
                {
                    // Nothing to do here.
                }

                private Func<ulong, AddedToScope, Task> AddedToScopeSender = null;
                private Func<ulong, RemovedFromScope, Task> RemovedFromScopeSender = null;
                private Dictionary<Type, object> ObjectSpawnedSenders = new Dictionary<Type, object>();
                private Dictionary<Type, object> ObjectSpawnedBroadcasters = new Dictionary<Type, object>();
                private Dictionary<Type, object> ObjectRefreshSenders = new Dictionary<Type, object>();
                private Dictionary<Type, object> ObjectWatchedSenders = new Dictionary<Type, object>();
                private Dictionary<Type, object> ObjectUnwatchedSenders = new Dictionary<Type, object>();
                private Dictionary<Type, Dictionary<string, object>> ObjectUpdatedSenders = new Dictionary<Type, Dictionary<string, object>>();
                private Dictionary<Type, Dictionary<string, object>> ObjectUpdatedBroadcasters = new Dictionary<Type, Dictionary<string, object>>();
                private Func<IEnumerable<ulong>, ObjectDespawned, Dictionary<ulong, Task>> ObjectDespawnedBroadcaster = null;
                private Func<IEnumerable<ulong>, ObjectAttached, Dictionary<ulong, Task>> ObjectAttachedBroadcaster = null;
                private Func<IEnumerable<ulong>, ObjectDetached, Dictionary<ulong, Task>> ObjectDetachedBroadcaster = null;
                private Func<IEnumerable<ulong>, ObjectTeleported, Dictionary<ulong, Task>> ObjectTeleportedBroadcaster = null;
                private Func<IEnumerable<ulong>, UInt, Dictionary<ulong, Task>> ObjectSpeedChangedBroadcaster = null;
                private Func<IEnumerable<ulong>, Enum<Direction>, Dictionary<ulong, Task>> ObjectOrientationChangedBroadcaster = null;
                private Func<IEnumerable<ulong>, ObjectMovementStarted, Dictionary<ulong, Task>> ObjectMovementStartedBroadcaster = null;
                private Func<IEnumerable<ulong>, ObjectMovementFinished, Dictionary<ulong, Task>> ObjectMovementFinishedBroadcaster = null;
                private Func<IEnumerable<ulong>, ObjectMovementCancelled, Dictionary<ulong, Task>> ObjectMovementCancelledBroadcaster = null;

                /// <summary>
                ///   Initializes the server-side protocol. This contains a
                ///   placeholder to register custom sub-classes of either
                ///   <see cref="MapObjectPrimaryModelClientSide"/> or
                ///   <see cref="MapObjectWatchedModelClientSide"/>.
                /// </summary>
                protected override void Initialize()
                {
                    base.Initialize();
                    AddedToScopeSender = MakeSender<AddedToScope>("Scope:Added");
                    RemovedFromScopeSender = MakeSender<RemovedFromScope>("Scope:Removed");
                    // ObjectSpawned<T> messages, as well as ObjectWatched<T>, ObjectUnwatched<T>,
                    // ObjectUpdated<T>,  ObjectRefresh<T>,... are sent differently.
                    ObjectDespawnedBroadcaster = MakeBroadcaster<ObjectDespawned>("Object:Despawned");
                    ObjectAttachedBroadcaster = MakeBroadcaster<ObjectAttached>("Object:Attached");
                    ObjectDetachedBroadcaster = MakeBroadcaster<ObjectDetached>("Object:Detached");
                    ObjectTeleportedBroadcaster = MakeBroadcaster<ObjectTeleported>("Object:Teleported");
                    ObjectSpeedChangedBroadcaster = MakeBroadcaster<UInt>("Object:Speed:Changed");
                    ObjectOrientationChangedBroadcaster = MakeBroadcaster<Enum<Direction>>("Object:Orientation:Changed");
                    ObjectMovementStartedBroadcaster = MakeBroadcaster<ObjectMovementStarted>("Object:Movement:Started");
                    ObjectMovementFinishedBroadcaster = MakeBroadcaster<ObjectMovementFinished>("Object:Movement:Finished");
                    ObjectMovementCancelledBroadcaster = MakeBroadcaster<ObjectMovementCancelled>("Object:Movement:Cancelled");
                }

                /// <summary>
                ///   Sends an <see cref="AddedToScope"/> message to a single client.
                ///   Meant to be sent when the connection is added to a new scope,
                ///   so that the client now loads / instantiates the given scope
                ///   accordingly.
                /// </summary>
                /// <param name="connection">The connection id to send this message to</param>
                /// <param name="scopePrefabIndex">The index of the prefab the scope is created from</param>
                /// <param name="scopeIndex">The index of the scope in the game server</param>
                /// <returns>A task, since this function is asynchronous</returns>
                public Task SendAddedToScope(ulong connection, uint scopePrefabIndex, uint scopeIndex)
                {
                    return AddedToScopeSender?.Invoke(connection, new AddedToScope() {
                        ScopePrefabIndex = scopePrefabIndex,
                        ScopeInstanceIndex = scopeIndex
                    });
                }

                /// <summary>
                ///   Sends a <see cref="RemovedFromScope"/> message to a single client.
                ///   Meant to be sent when the connection is removed from a scope,
                ///   so that the client now cleans up all the scope on its side.
                /// </summary>
                /// <param name="connection">The connection id to send this message to</param>
                /// <param name="scopeIndex">The index of the scope in the game server</param>
                /// <returns>A task, since this function is asynchronous</returns>
                public Task SendRemovedFromScope(ulong connection, uint scopeIndex)
                {
                    return RemovedFromScopeSender?.Invoke(connection, new RemovedFromScope() {
                        ScopeInstanceIndex = scopeIndex
                    });
                }

                /// <summary>
                ///   Broadcasts an <see cref="ObjectSpawned{T}"/> message. Meant
                ///   to be used to notify all the connections watching a given scope,
                ///   when a new object is added to it.
                /// </summary>
                /// <typeparam name="ModelClass">The primary model class behaviour this message is related to</typeparam>
                /// <typeparam name="T">The data type for the model</typeparam>
                /// <param name="connections">The connection ids to send this message to. Recommended: clone this, to avoid race conditions</param>
                /// <param name="scopeIndex">The index of the scope the object is being spawned into</param>
                /// <param name="objectPrefabIndex">The index of the prefab the object is created from</param>
                /// <param name="objectIndex">The index of the object inside the given scope</param>
                /// <param name="data">The data for the model. Recommended: clone this, to avoid race conditions</param>
                /// <param name="orientation">The object's orientation</param>
                /// <param name="speed">The object's speed</param>
                /// <param name="attached">Whether the object is attached to a map or not</param>
                /// <param name="mapIndex">The map the object belongs to - ignore it if not attached (it will be 0 but without meaning)</param>
                /// <param name="x">The x position of the object - ignore it if not attached (it will be 0 but without meaning)</param>
                /// <param name="y">The y position of the object - ignore it if not attached (it will be 0 but without meaning)</param>
                /// <param name="movement">The object's movement, if any - ignore it if not attached (it will be null)</param>
                /// <returns>A task, since this function is asynchronous</returns>
                public Task BroadcastSpawned<ModelClass, T>(IEnumerable<ulong> connections, uint scopeIndex, uint objectPrefabIndex, uint objectIndex, T data, Direction orientation, uint speed, bool attached, byte mapIndex, ushort x, ushort y, Direction? movement)
                    where ModelClass : MapObjectPrimaryModel<T>
                    where T : ISerializable, new()
                {
                    Type type = typeof(ModelClass);
                    if (!ObjectSpawnedBroadcasters.ContainsKey(type))
                    {
                        ObjectSpawnedBroadcasters[type] = MakeBroadcaster<ObjectSpawned<T>>($"Object:Spawned:{type.FullName}");
                    }
                    Func<IEnumerable<ulong>, ObjectSpawned<T>, Dictionary<ulong, Task>> broadcaster = 
                        (Func<IEnumerable<ulong>, ObjectSpawned<T>, Dictionary<ulong, Task>>)ObjectSpawnedBroadcasters[type];
                    return UntilBroadcastIsDone(broadcaster?.Invoke(connections, new ObjectSpawned<T>() {
                        ScopeInstanceIndex = scopeIndex,
                        ObjectPrefabIndex = objectPrefabIndex,
                        ObjectInstanceIndex = objectIndex,
                        Data = data,
                        Orientation = orientation,
                        Speed = speed,
                        Attached = attached,
                        MapIndex = mapIndex,
                        X = x,
                        Y = y,
                        Movement = movement
                    }));
                }

                /// <summary>
                ///   Sends an <see cref="ObjectSpawned{T}"/> message to a single client.
                ///   Meant to be used to notify a single connection about an object (this
                ///   stands for new connections, and this message will be sent to that
                ///   connection for each object in the same scope).
                /// </summary>
                /// <typeparam name="ModelClass">The primary model class behaviour this message is related to</typeparam>
                /// <typeparam name="T">The data type for the model</typeparam>
                /// <param name="connection">The connection id to send this message to</param>
                /// <param name="scopeIndex">The index of the scope the object is being spawned into</param>
                /// <param name="objectPrefabIndex">The index of the prefab the object is created from</param>
                /// <param name="objectIndex">The index of the object inside the given scope</param>
                /// <param name="data">The data for the model. Recommended: clone this, to avoid race conditions</param>
                /// <param name="orientation">The object's orientation</param>
                /// <param name="speed">The object's speed</param>
                /// <param name="attached">Whether the object is attached to a map or not</param>
                /// <param name="mapIndex">The map the object belongs to - ignore it if not attached (it will be 0 but without meaning)</param>
                /// <param name="x">The x position of the object - ignore it if not attached (it will be 0 but without meaning)</param>
                /// <param name="y">The y position of the object - ignore it if not attached (it will be 0 but without meaning)</param>
                /// <param name="movement">The object's movement, if any - ignore it if not attached (it will be null)</param>
                /// <returns>A task, since this function is asynchronous</returns>
                public Task SendSpawned<ModelClass, T>(ulong connection, uint scopeIndex, uint objectPrefabIndex, uint objectIndex, T data, Direction orientation, uint speed, bool attached, byte mapIndex, ushort x, ushort y, Direction? movement)
                    where ModelClass : MapObjectPrimaryModel<T>
                    where T : ISerializable, new()
                {
                    Type type = typeof(ModelClass);
                    if (!ObjectSpawnedSenders.ContainsKey(type))
                    {
                        ObjectSpawnedSenders[type] = MakeSender<ObjectSpawned<T>>($"Object:Spawned:{type.FullName}");
                    }
                    Func<ulong, ObjectSpawned<T>, Task> sender = (Func<ulong, ObjectSpawned<T>, Task>)ObjectSpawnedSenders[type];
                    return sender(connection, new ObjectSpawned<T>() {
                        ScopeInstanceIndex = scopeIndex,
                        ObjectPrefabIndex = objectPrefabIndex,
                        ObjectInstanceIndex = objectIndex,
                        Data = data,
                        Orientation = orientation,
                        Speed = speed,
                        Attached = attached,
                        MapIndex = mapIndex,
                        X = x,
                        Y = y,
                        Movement = movement
                    });
                }

                /// <summary>
                ///   Sends an <see cref="ObjectRefresh{T}"/> message to a single client.
                ///   Meant to be used to notify a single connection about an object (this
                ///   stands for a connection that is updated in the way it views the data
                ///   of all of the available objects, and thus this message will be sent
                ///   to that connection for each object in the same scope that should be
                ///   considered needing certain type of refresh).
                /// </summary>
                /// <typeparam name="ModelClass">The primary model class behaviour this message is related to</typeparam>
                /// <typeparam name="T">The data type for the model</typeparam>
                /// <param name="connection">The connection id to send this message to</param>
                /// <param name="scopeIndex">The index of the scope the object belongs to</param>
                /// <param name="objectIndex">The index of the object inside the given scope</param>
                /// <param name="data">The new data for the object</param>
                /// <returns>A task, since this function is asynchronous</returns>
                public Task SendRefresh<ModelClass, T>(ulong connection, uint scopeIndex, uint objectIndex, T data)
                    where ModelClass : MapObjectPrimaryModel<T>
                    where T : ISerializable, new()
                {
                    Type type = typeof(ModelClass);
                    if (!ObjectRefreshSenders.ContainsKey(type))
                    {
                        ObjectRefreshSenders[type] = MakeSender<ObjectRefresh<T>>($"Object:Refresh:{type.FullName}");
                    }
                    Func<ulong, ObjectRefresh<T>, Task> sender = (Func<ulong, ObjectRefresh<T>, Task>)ObjectRefreshSenders[type];
                    return sender(connection, new ObjectRefresh<T>() {
                        ScopeInstanceIndex = scopeIndex,
                        ObjectInstanceIndex = objectIndex,
                        Data = data
                    });
                }

                /// <summary>
                ///   Broadcasts an <see cref="ObjectDespawned"/> message. Meant
                ///   to be used to notify all the connections watching a given scope,
                ///   when an existing object is removed from it.
                /// </summary>
                /// <param name="connections">The connection ids to send this message to. Recommended: clone this, to avoid race conditions</param>
                /// <param name="scopeIndex">The index of the scope the object is despawned from</param>
                /// <param name="objectIndex">The id of the object being despawned</param>
                /// <returns>A task, since this function is asynchronous</returns>
                public Task BroadcastDespawned(IEnumerable<ulong> connections, uint scopeIndex, uint objectIndex)
                {
                    return UntilBroadcastIsDone(ObjectDespawnedBroadcaster?.Invoke(connections, new ObjectDespawned() {
                        ScopeInstanceIndex = scopeIndex,
                        ObjectInstanceIndex = objectIndex
                    }));
                }

                /// <summary>
                ///   Sends an <see cref="ObjectWatched{T}"/> message to a single client.
                ///   Meant to observe the object in a particular way (e.g. owned data,
                ///   like health / status, or close-watch data, like chests and player
                ///   inventories).
                /// </summary>
                /// <typeparam name="ModelClass">The watched model class behaviour this message is related to</typeparam>
                /// <typeparam name="T">The data type for the model</typeparam>
                /// <param name="connection">The connection id to send this message to</param>
                /// <param name="scopeIndex">The index of the scope the object belongs to</param>
                /// <param name="objectIndex">The index of the object inside the given scope</param>
                /// <param name="data">The current data for the object</param>
                /// <returns>A task, since this function is asynchronous</returns>
                public Task SendWatched<ModelClass, T>(ulong connection, uint scopeIndex, uint objectIndex, T data)
                    where ModelClass : MapObjectWatchedModel<T>
                    where T : ISerializable, new()
                {
                    Type type = typeof(ModelClass);
                    if (!ObjectWatchedSenders.ContainsKey(type))
                    {
                        ObjectWatchedSenders[type] = MakeSender<ObjectWatched<T>>($"Object:Watched:{type.FullName}");
                    }
                    Func<ulong, ObjectWatched<T>, Task> sender = (Func<ulong, ObjectWatched<T>, Task>)ObjectWatchedSenders[type];
                    return sender(connection, new ObjectWatched<T>() {
                        ScopeInstanceIndex = scopeIndex,
                        ObjectInstanceIndex = objectIndex,
                        Data = data
                    });
                }

                /// <summary>
                ///   Sends an <see cref="ObjectUnwatched"/> message to a single client.
                ///   Meant to un-observe the object in a particular way it was set to
                ///   observe by using <see cref="SendWatched{ModelClass, T}(ulong, uint, uint, T)"/>.
                /// </summary>
                /// <typeparam name="ModelClass">The watched model class behaviour this message is related to</typeparam>
                /// <typeparam name="T">The data type for the model</typeparam>
                /// <param name="connection">The connection id to send this message to</param>
                /// <param name="scopeIndex">The index of the scope the object belongs to</param>
                /// <param name="objectIndex">The index of the object inside the given scope</param>
                /// <returns>A task, since this function is asynchronous</returns>
                /// <remarks>No data will be sent, aside from the indices</remarks>
                public Task SendUnwatched<ModelClass, T>(ulong connection, uint scopeIndex, uint objectIndex)
                    where ModelClass : MapObjectWatchedModel<T>
                    where T : ISerializable, new()
                {
                    Type type = typeof(ModelClass);
                    if (!ObjectUnwatchedSenders.ContainsKey(type))
                    {
                        ObjectUnwatchedSenders[type] = MakeSender<ObjectUnwatched>($"Object:Unwatched:{type.FullName}");
                    }
                    Func<ulong, ObjectUnwatched, Task> sender = (Func<ulong, ObjectUnwatched, Task>)ObjectUnwatchedSenders[type];
                    return sender(connection, new ObjectUnwatched() {
                        ScopeInstanceIndex = scopeIndex,
                        ObjectInstanceIndex = objectIndex
                    });
                }

                /// <summary>
                ///   Broadcasts an <see cref="ObjectUpdated{VT}"/> message. Meant to tell
                ///   when a single property changed in a given model.
                /// </summary>
                /// <typeparam name="ModelClass">The primary model class behaviour this message is related to</typeparam>
                /// <typeparam name="MT">The data type for the model</typeparam>
                /// <typeparam name="VT">The data type for the property</typeparam>
                /// <param name="connections">The connection ids to send this message to. Recommended: clone this, to avoid race conditions</param>
                /// <param name="scopeIndex">The index of the scope the object belongs to</param>
                /// <param name="objectIndex">The index of the object inside the given scope</param>
                /// <param name="property">The property being updated</param>
                /// <param name="value">The new value of the property. Depending on the type, it may be some kind of delta value, and not a full replacement</param>
                /// <returns>A task, since this function is asynchronous</returns>
                private Task BroadcastModelUpdate<T, VT>(IEnumerable<ulong> connections, uint scopeIndex, uint objectIndex, string property, VT value)
                    where VT : ISerializable, new()
                {
                    Type type = typeof(T);
                    Type vtType = typeof(VT);

                    if (!ObjectUpdatedBroadcasters.ContainsKey(type))
                    {
                        ObjectUpdatedBroadcasters[type] = new Dictionary<string, object>();
                    }

                    if (!ObjectUpdatedBroadcasters[type].ContainsKey(property))
                    {
                        ObjectUpdatedBroadcasters[type][property] = MakeBroadcaster<ObjectUpdated<VT>>($"Object:Updated:{type.FullName}.{property}");
                    }

                    Func<IEnumerable<ulong>, ObjectUpdated<VT>, Dictionary<ulong, Task>> broadcaster =
                        (Func<IEnumerable<ulong>, ObjectUpdated<VT>, Dictionary<ulong, Task>>)ObjectUpdatedBroadcasters[type][property];
                    return UntilBroadcastIsDone(broadcaster.Invoke(connections, new ObjectUpdated<VT>() {
                        ScopeInstanceIndex = scopeIndex,
                        ObjectInstanceIndex = objectIndex,
                        Value = value
                    }));
                }

                /// <summary>
                ///   Broadcasts an <see cref="ObjectUpdated{VT}"/> message. Meant to tell
                ///   when a single property changed in a given primary model.
                /// </summary>
                /// <typeparam name="ModelClass">The primary model class behaviour this message is related to</typeparam>
                /// <typeparam name="MT">The data type for the model</typeparam>
                /// <typeparam name="VT">The data type for the property</typeparam>
                /// <param name="connections">The connection ids to send this message to. Recommended: clone this, to avoid race conditions</param>
                /// <param name="scopeIndex">The index of the scope the object belongs to</param>
                /// <param name="objectIndex">The index of the object inside the given scope</param>
                /// <param name="property">The property being updated</param>
                /// <param name="value">The new value of the property. Depending on the type, it may be some kind of delta value, and not a full replacement</param>
                /// <returns>A task, since this function is asynchronous</returns>
                public Task BroadcastPrimaryModelUpdate<ModelClass, MT, VT>(IEnumerable<ulong> connections, uint scopeIndex, uint objectIndex, string property, VT value)
                    where ModelClass : MapObjectPrimaryModel<MT>
                    where MT : ISerializable, new()
                    where VT : ISerializable, new()
                {
                    return BroadcastModelUpdate<ModelClass, VT>(connections, scopeIndex, objectIndex, property, value);
                }

                /// <summary>
                ///   Broadcasts an <see cref="ObjectUpdated{VT}"/> message. Meant to tell
                ///   when a single property changed in a given watched model, and the
                ///   property is not strictly "ownership"-related, but many connections
                ///   may be watching it at the same time.
                /// </summary>
                /// <typeparam name="ModelClass">The watched model class behaviour this message is related to</typeparam>
                /// <typeparam name="MT">The data type for the model</typeparam>
                /// <typeparam name="VT">The data type for the property</typeparam>
                /// <param name="connections">The connection ids to send this message to. Recommended: clone this, to avoid race conditions</param>
                /// <param name="scopeIndex">The index of the scope the object belongs to</param>
                /// <param name="objectIndex">The index of the object inside the given scope</param>
                /// <param name="property">The property being updated</param>
                /// <param name="value">The new value of the property. Depending on the type, it may be some kind of delta value, and not a full replacement</param>
                /// <returns>A task, since this function is asynchronous</returns>
                public Task BroadcastWatchedModelUpdate<ModelClass, MT, VT>(IEnumerable<ulong> connections, uint scopeIndex, uint objectIndex, string property, VT value)
                    where ModelClass : MapObjectWatchedModel<MT>
                    where MT : ISerializable, new()
                    where VT : ISerializable, new()
                {
                    return BroadcastModelUpdate<ModelClass, VT>(connections, scopeIndex, objectIndex, property, value);
                }

                private Task SendModelUpdate<T, VT>(ulong connection, uint scopeIndex, uint objectIndex, string property, VT value)
                    where VT : ISerializable, new()
                {
                    Type type = typeof(T);
                    Type vtType = typeof(VT);

                    if (!ObjectUpdatedSenders.ContainsKey(type))
                    {
                        ObjectUpdatedSenders[type] = new Dictionary<string, object>();
                    }

                    if (!ObjectUpdatedSenders[type].ContainsKey(property))
                    {
                        ObjectUpdatedSenders[type][property] = MakeSender<ObjectUpdated<VT>>($"Object:Updated:{type.FullName}.{property}");
                    }

                    Func<ulong, ObjectUpdated<VT>, Task> sender =
                        (Func<ulong, ObjectUpdated<VT>, Task>)ObjectUpdatedSenders[type][property];
                    return sender.Invoke(connection, new ObjectUpdated<VT>() {
                        ScopeInstanceIndex = scopeIndex,
                        ObjectInstanceIndex = objectIndex,
                        Value = value
                    });
                }

                /// <summary>
                ///   Sends an <see cref="ObjectUpdated{VT}"/> message to a single client.
                ///   Meant to tell when a single property changed in a given watched model,
                ///   in "ownership"-related properties.
                /// </summary>
                /// <typeparam name="ModelClass">The watched model class behaviour this message is related to</typeparam>
                /// <typeparam name="MT">The data type for the model</typeparam>
                /// <typeparam name="VT">The data type for the property</typeparam>
                /// <param name="connection">The connection id to send this message to</param>
                /// <param name="scopeIndex">The index of the scope the object belongs to</param>
                /// <param name="objectIndex">The index of the object inside the given scope</param>
                /// <param name="property">The property being updated</param>
                /// <param name="value">The new value of the property. Depending on the type, it may be some kind of delta value, and not a full replacement</param>
                /// <returns>A task, since this function is asynchronous</returns>
                public Task SendWatchedModelUpdate<ModelClass, MT, VT>(ulong connection, uint scopeIndex, uint objectIndex, string property, VT value)
                    where ModelClass : MapObjectWatchedModel<MT>
                    where MT : ISerializable, new()
                    where VT : ISerializable, new()
                {
                    return SendModelUpdate<ModelClass, VT>(connection, scopeIndex, objectIndex, property, value);
                }

                /// <summary>
                ///   broadcasts an <see cref="ObjectAttached"/> message. Meant to tell when
                ///   an object was attached to a map.
                /// </summary>
                /// <param name="connections">The connection ids to send this message to. Recommended: clone this, to avoid race conditions</param>
                /// <param name="scopeIndex">The index of the scope the object belongs to</param>
                /// <param name="objectIndex">The index of the object inside the given scope</param>
                /// <param name="map">The index of the map inside the given scope</param>
                /// <param name="x">The new x-position of the object inside the new map</param>
                /// <param name="y">The new y-position of the object inside the new map</param>
                /// <returns>A task, since this function is asynchronous</returns>
                public Task BroadcastObjectAttached(IEnumerable<ulong> connections, uint scopeIndex, uint objectIndex, byte map, ushort x, ushort y)
                {
                    return UntilBroadcastIsDone(ObjectAttachedBroadcaster?.Invoke(connections, new ObjectAttached() { 
                        ScopeInstanceIndex = scopeIndex,
                        ObjectInstanceIndex = objectIndex,
                        MapIndex = map,
                        TargetX = x,
                        TargetY = y
                    }));
                }

                /// <summary>
                ///   broadcasts an <see cref="ObjectDetached"/> message. Meant to tell when
                ///   an object was detached from a map.
                /// </summary>
                /// <param name="connections">The connection ids to send this message to. Recommended: clone this, to avoid race conditions</param>
                /// <param name="scopeIndex">The index of the scope the object belongs to</param>
                /// <param name="objectIndex">The index of the object inside the given scope</param>
                public Task BroadcastObjectDetached(IEnumerable<ulong> connections, uint scopeIndex, uint objectIndex)
                {
                    return UntilBroadcastIsDone(ObjectDetachedBroadcaster?.Invoke(connections, new ObjectDetached() {
                        ScopeInstanceIndex = scopeIndex,
                        ObjectInstanceIndex = objectIndex
                    }));
                }

                /// <summary>
                ///   broadcasts an <see cref="ObjectTeleported"/> message. Meant to tell when
                ///   an object was teleported inside the same map it belonged to.
                /// </summary>
                /// <param name="connections">The connection ids to send this message to. Recommended: clone this, to avoid race conditions</param>
                /// <param name="scopeIndex">The index of the scope the object belongs to</param>
                /// <param name="objectIndex">The index of the object inside the given scope</param>
                /// <param name="x">The new x-position of the object inside the same map</param>
                /// <param name="y">The new y-position of the object inside the same map</param>
                /// <returns>A task, since this function is asynchronous</returns>
                public Task BroadcastObjectTeleported(IEnumerable<ulong> connections, uint scopeIndex, uint objectIndex, ushort x, ushort y)
                {
                    return UntilBroadcastIsDone(ObjectTeleportedBroadcaster?.Invoke(connections, new ObjectTeleported() {
                        ScopeInstanceIndex = scopeIndex,
                        ObjectInstanceIndex = objectIndex,
                        TargetX = x,
                        TargetY = y
                    }));
                }

                /// <summary>
                ///   Broadcasts an <see cref="ObjectSpeedChanged"/> message. Meant to tell when
                ///   an object's speed was changed.
                /// </summary>
                /// <param name="connections">The connection ids to send this message to. Recommended: clone this, to avoid race conditions</param>
                /// <param name="scopeIndex">The index of the scope the object belongs to</param>
                /// <param name="objectIndex">The index of the object inside the given scope</param>
                /// <param name="speed">The new speed</param>
                /// <returns>A task, since this function is asynchronous</returns>
                public Task BroadcastSpeedChanged(IEnumerable<ulong> connections, uint scopeIndex, uint objectIndex, uint speed)
                {
                    return UntilBroadcastIsDone(ObjectSpeedChangedBroadcaster?.Invoke(connections, (UInt)speed));
                }

                /// <summary>
                ///   Broadcasts an <see cref="ObjectOrientationChanged"/> message. Meant to tell when
                ///   an object's orientation was changed.
                /// </summary>
                /// <param name="connections">The connection ids to send this message to. Recommended: clone this, to avoid race conditions</param>
                /// <param name="scopeIndex">The index of the scope the object belongs to</param>
                /// <param name="objectIndex">The index of the object inside the given scope</param>
                /// <param name="orientation">The new orientation</param>
                /// <returns>A task, since this function is asynchronous</returns>
                public Task BroadcastOrientationChanged(IEnumerable<ulong> connections, uint scopeIndex, uint objectIndex, Direction orientation)
                {
                    return UntilBroadcastIsDone(ObjectOrientationChangedBroadcaster?.Invoke(connections, (Enum<Direction>)orientation));
                }

                /// <summary>
                ///   Broadcasts an <see cref="ObjectMovementStarted"/> message. Meant to tell when
                ///   an object has started moving.
                /// </summary>
                /// <param name="connections">The connection ids to send this message to. Recommended: clone this, to avoid race conditions</param>
                /// <param name="scopeIndex">The index of the scope the object belongs to</param>
                /// <param name="objectIndex">The index of the object inside the given scope</param>
                /// <param name="startX">The start x-position of the object starting the movement</param>
                /// <param name="startY">The start y-position of the object starting the movement</param>
                /// <param name="movement">The direction of the new movement</param>
                /// <returns>A task, since this function is asynchronous</returns>
                public Task BroadcastMovementStarted(IEnumerable<ulong> connections, uint scopeIndex, uint objectIndex, ushort startX, ushort startY, Direction movement)
                {
                    return UntilBroadcastIsDone(ObjectMovementStartedBroadcaster?.Invoke(connections, new ObjectMovementStarted() {
                        ScopeInstanceIndex = scopeIndex,
                        ObjectInstanceIndex = objectIndex,
                        Direction = movement,
                        StartX = startX,
                        StartY = startY
                    }));
                }

                /// <summary>
                ///   Broadcasts an <see cref="ObjectMovementFinished"/> message. Meant to tell when
                ///   an object has finished moving.
                /// </summary>
                /// <param name="connections">The connection ids to send this message to. Recommended: clone this, to avoid race conditions</param>
                /// <param name="scopeIndex">The index of the scope the object belongs to</param>
                /// <param name="objectIndex">The index of the object inside the given scope</param>
                /// <param name="endX">The end x-position of the object</param>
                /// <param name="endY">The end y-position of the object</param>
                /// <returns>A task, since this function is asynchronous</returns>
                public Task BroadcastMovementFinished(IEnumerable<ulong> connections, uint scopeIndex, uint objectIndex, ushort endX, ushort endY)
                {
                    return UntilBroadcastIsDone(ObjectMovementFinishedBroadcaster?.Invoke(connections, new ObjectMovementFinished() {
                        ScopeInstanceIndex = scopeIndex,
                        ObjectInstanceIndex = objectIndex,
                        EndX = endX,
                        EndY = endY
                    }));
                }

                /// <summary>
                ///   Broadcasts an <see cref="ObjectMovementCancelled"/> message. Meant to tell when
                ///   an object has cancelled its current movement.
                /// </summary>
                /// <param name="connections">The connection ids to send this message to. Recommended: clone this, to avoid race conditions</param>
                /// <param name="scopeIndex">The index of the scope the object belongs to</param>
                /// <param name="objectIndex">The index of the object inside the given scope</param>
                /// <param name="revertToX">The start x-position of the object cancelling the movement</param>
                /// <param name="revertToY">The start y-position of the object cancelling the movement</param>
                /// <returns>A task, since this function is asynchronous</returns>
                public Task BroadcastMovementCancelled(IEnumerable<ulong> connections, uint scopeIndex, uint objectIndex, ushort revertToX, ushort revertToY)
                {
                    return UntilBroadcastIsDone(ObjectMovementCancelledBroadcaster?.Invoke(connections, new ObjectMovementCancelled()
                    {
                        ScopeInstanceIndex = scopeIndex,
                        ObjectInstanceIndex = objectIndex,
                        RevertToX = revertToX,
                        RevertToY = revertToY
                    }));
                }
            }
        }
    }
}

