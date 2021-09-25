using AlephVault.Unity.Binary;
using GameMeanMachine.Unity.NetRose.Authoring.Behaviours.Models.Entities.Objects;
using GameMeanMachine.Unity.NetRose.Types.Protocols.Messages;
using GameMeanMachine.Unity.WindRose.Types;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


namespace GameMeanMachine.Unity.NetRose
{
    namespace Authoring
    {
        namespace Protocols
        {
            /// <summary>
            ///   All of the <see cref="NetRoseProtocolServerSide{Definition}"/>
            ///   instances will implement this interface, and will be able to
            ///   send messages to the clients appropriately (actually, these
            ///   function will be used by the scopes - not by the objects
            ///   themselves), strictly related to the NetRose movements and
            ///   the data updates.
            /// </summary>
            public interface INetRoseProtocolServerSideSender
            {
                // Scope/connection messages.

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
                Task SendAddedToScope(ulong connection, uint scopePrefabIndex, uint scopeIndex);

                /// <summary>
                ///   Sends a <see cref="RemovedFromScope"/> message to a single client.
                ///   Meant to be sent when the connection is removed from a scope,
                ///   so that the client now cleans up all the scope on its side.
                /// </summary>
                /// <param name="connection">The connection id to send this message to</param>
                /// <param name="scopeIndex">The index of the scope in the game server</param>
                /// <returns>A task, since this function is asynchronous</returns>
                Task SendRemovedFromScope(ulong connection, uint scopeIndex);

                // Object spawn/despawn/watch/unwatch/update messages.

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
                Task BroadcastSpawned<ModelClass, T>(
                    IEnumerable<ulong> connections,
                    uint scopeIndex, uint objectPrefabIndex, uint objectIndex,
                    T data, Direction orientation, uint speed,
                    bool attached, byte mapIndex, ushort x, ushort y, Direction? movement
                ) where ModelClass : MapObjectPrimaryModel<T> where T : ISerializable, new();

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
                Task SendSpawned<ModelClass, T>(
                    ulong connection,
                    uint scopeIndex, uint objectPrefabIndex, uint objectIndex,
                    T data, Direction orientation, uint speed,
                    bool attached, byte mapIndex, ushort x, ushort y, Direction? movement
                ) where ModelClass : MapObjectPrimaryModel<T> where T : ISerializable, new();

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
                Task SendRefresh<ModelClass, T>(
                    ulong connection, uint scopeIndex, uint objectIndex, T data
                ) where ModelClass : MapObjectPrimaryModel<T> where T : ISerializable, new();

                /// <summary>
                ///   Broadcasts an <see cref="ObjectDespawned"/> message. Meant
                ///   to be used to notify all the connections watching a given scope,
                ///   when an existing object is removed from it.
                /// </summary>
                /// <param name="connections">The connection ids to send this message to. Recommended: clone this, to avoid race conditions</param>
                /// <param name="scopeIndex">The index of the scope the object is despawned from</param>
                /// <param name="objectIndex">The id of the object being despawned</param>
                /// <returns>A task, since this function is asynchronous</returns>
                Task BroadcastDespawned(
                    IEnumerable<ulong> connections, uint scopeIndex, uint objectIndex
                );

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
                Task SendWatched<ModelClass, T>(
                    ulong connection, uint scopeIndex, uint objectIndex, T data
                ) where ModelClass : MapObjectWatchedModel<T> where T : ISerializable, new();

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
                Task SendUnwatched<ModelClass, T>(
                    ulong connection, uint scopeIndex, uint objectIndex
                ) where ModelClass : MapObjectWatchedModel<T> where T : ISerializable, new();

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
                Task BroadcastPrimaryModelUpdate<ModelClass, MT, VT>(
                    IEnumerable<ulong> connections, uint scopeIndex, uint objectIndex, string property, VT value
                ) where ModelClass : MapObjectPrimaryModel<MT> where MT : ISerializable, new() where VT : ISerializable, new();

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
                Task BroadcastWatchedModelUpdate<ModelClass, MT, VT>(
                    IEnumerable<ulong> connections, uint scopeIndex, uint objectIndex, string property, VT value
                ) where ModelClass : MapObjectWatchedModel<MT> where MT : ISerializable, new() where VT : ISerializable, new();

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
                Task SendWatchedModelUpdate<ModelClass, MT, VT>(
                    ulong connection, uint scopeIndex, uint objectIndex, string property, VT value
                ) where ModelClass : MapObjectWatchedModel<MT> where MT : ISerializable, new() where VT : ISerializable, new();

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
                Task BroadcastObjectAttached(
                    IEnumerable<ulong> connections, uint scopeIndex, uint objectIndex, byte map, ushort x, ushort y
                );

                /// <summary>
                ///   broadcasts an <see cref="ObjectDetached"/> message. Meant to tell when
                ///   an object was detached from a map.
                /// </summary>
                /// <param name="connections">The connection ids to send this message to. Recommended: clone this, to avoid race conditions</param>
                /// <param name="scopeIndex">The index of the scope the object belongs to</param>
                /// <param name="objectIndex">The index of the object inside the given scope</param>
                Task BroadcastObjectDetached(
                    IEnumerable<ulong> connections, uint scopeIndex, uint objectIndex
                );

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
                Task BroadcastObjectTeleported(
                    IEnumerable<ulong> connections, uint scopeIndex, uint objectIndex, ushort x, ushort y
                );

                /// <summary>
                ///   Broadcasts an <see cref="ObjectSpeedChanged"/> message. Meant to tell when
                ///   an object's speed was changed.
                /// </summary>
                /// <param name="connections">The connection ids to send this message to. Recommended: clone this, to avoid race conditions</param>
                /// <param name="scopeIndex">The index of the scope the object belongs to</param>
                /// <param name="objectIndex">The index of the object inside the given scope</param>
                /// <param name="speed">The new speed</param>
                /// <returns>A task, since this function is asynchronous</returns>
                Task BroadcastSpeedChanged(
                    IEnumerable<ulong> connections, uint scopeIndex, uint objectIndex, uint speed
                );

                /// <summary>
                ///   Broadcasts an <see cref="ObjectOrientationChanged"/> message. Meant to tell when
                ///   an object's orientation was changed.
                /// </summary>
                /// <param name="connections">The connection ids to send this message to. Recommended: clone this, to avoid race conditions</param>
                /// <param name="scopeIndex">The index of the scope the object belongs to</param>
                /// <param name="objectIndex">The index of the object inside the given scope</param>
                /// <param name="orientation">The new orientation</param>
                /// <returns>A task, since this function is asynchronous</returns>
                Task BroadcastOrientationChanged(
                    IEnumerable<ulong> connections, uint scopeIndex, uint objectIndex, Direction orientation
                );

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
                Task BroadcastMovementStarted(
                    IEnumerable<ulong> connections, uint scopeIndex, uint objectIndex, ushort startX, ushort startY, Direction movement
                );

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
                Task BroadcastMovementFinished(
                    IEnumerable<ulong> connections, uint scopeIndex, uint objectIndex, ushort endX, ushort endY
                );

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
                Task BroadcastMovementCancelled(
                    IEnumerable<ulong> connections, uint scopeIndex, uint objectIndex, ushort revertToX, ushort revertToY
                );
            }
        }
    }
}
