using System;
using AlephVault.Unity.Support.Generic.Vendor.IUnified.Authoring.Types;
using GameMeanMachine.Unity.NetRose.Authoring.Protocols;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.World;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using GameMeanMachine.Unity.WindRose.Types;
using GameMeanMachine.Unity.NetRose.Authoring.Behaviours.Models.Entities.Objects;
using AlephVault.Unity.Binary;

namespace GameMeanMachine.Unity.NetRose
{
    namespace Authoring
    {
        namespace Behaviours
        {
            namespace Server
            {
                namespace World
                {
                    [RequireComponent(typeof(Scope))]
                    public partial class ScopeServerSide : MonoBehaviour
                    {
                        // Only the messages are defined here. More complex
                        // methods are defined later.

                        // Sends AddedToScope to a single connection.
                        private Task SendAddedToScope(ulong connection)
                        {
                            return sender.Result.SendAddedToScope(connection, prefabIndex, id);
                        }

                        // Sends RemovedFromScope to a single connection.
                        private Task SendRemovedFromScope(ulong connection)
                        {
                            return sender.Result.SendRemovedFromScope(connection, id);
                        }

                        // Broadcasts ObjectSpawned to all the connections
                        // (actually, a split will be used to broadcast and
                        // different connections may receive different
                        // actual messages).
                        private Task BroadcastSpawned<ModelClass, T>(
                            IEnumerable<ulong> connections,
                            uint objectPrefabIndex, uint objectIndex,
                            T data, Direction orientation, uint speed,
                            bool attached, byte mapIndex, ushort x, ushort y,
                            Direction? movement
                        ) where ModelClass : MapObjectPrimaryModel<T> where T : ISerializable, new()
                        {
                            return sender.Result.BroadcastSpawned<ModelClass, T>(
                                connections, id, objectPrefabIndex, objectIndex,
                                data, orientation, speed, attached, mapIndex, x, y,
                                movement
                            );
                        }

                        // Sends ObjectSpawned to a single connection.
                        private Task SendSpawned<ModelClass, T>(
                            ulong connection,
                            uint objectPrefabIndex, uint objectIndex,
                            T data, Direction orientation, uint speed,
                            bool attached, byte mapIndex, ushort x, ushort y, Direction? movement
                        ) where ModelClass : MapObjectPrimaryModel<T> where T : ISerializable, new()
                        {
                            return sender.Result.SendSpawned<ModelClass, T>(
                                connection, id, objectPrefabIndex, objectIndex,
                                data, orientation, speed, attached, mapIndex, x, y,
                                movement
                            );
                        }

                        // Sends ObjectRefresh to a single connection.
                        private Task SendRefresh<ModelClass, T>(
                            ulong connection, uint objectIndex, T data
                        ) where ModelClass : MapObjectPrimaryModel<T> where T : ISerializable, new()
                        {
                            return sender.Result.SendRefresh<ModelClass, T>(connection, id, objectIndex, data);
                        }

                        // Broadcasts ObjectDespawned to all the connections.
                        private Task BroadcastDespawned(
                            IEnumerable<ulong> connections, uint objectIndex
                        )
                        {
                            return sender.Result.BroadcastDespawned(connections, id, objectIndex);
                        }

                        // Sends ObjectWatched to a single connection.
                        private Task SendWatched<ModelClass, T>(
                            ulong connection, uint objectIndex, T data
                        ) where ModelClass : MapObjectWatchedModel<T> where T : ISerializable, new()
                        {
                            return sender.Result.SendWatched<ModelClass, T>(connection, id, objectIndex, data);
                        }

                        // Sends ObjectUnwatched to a single connection.
                        private Task SendUnwatched<ModelClass, T>(
                            ulong connection, uint objectIndex
                        ) where ModelClass : MapObjectWatchedModel<T> where T : ISerializable, new()
                        {
                            return sender.Result.SendUnwatched<ModelClass, T>(connection, id, objectIndex);
                        }

                        // Broadcasts ObjectUpdated to all the connections.
                        // This, for a primary model property.
                        private Task BroadcastPrimaryModelUpdate<ModelClass, MT, VT>(
                            IEnumerable<ulong> connections, uint objectIndex, string property, VT value
                        ) where ModelClass : MapObjectPrimaryModel<MT> where MT : ISerializable, new() where VT : ISerializable, new()
                        {
                            return sender.Result.BroadcastPrimaryModelUpdate<ModelClass, MT, VT>(
                                connections, id, objectIndex, property, value
                            );
                        }

                        // Sends ObjectUpdated to a single connection.
                        // This, for a watched model property.
                        private Task SendWatchedModelUpdate<ModelClass, MT, VT>(
                            ulong connection, uint objectIndex, string property, VT value
                        ) where ModelClass : MapObjectWatchedModel<MT> where MT : ISerializable, new() where VT : ISerializable, new()
                        {
                            return sender.Result.SendWatchedModelUpdate<ModelClass, MT, VT>(
                                connection, id, objectIndex, property, value
                            );
                        }

                        // Sends ObjectAttached to all the connections.
                        private Task BroadcastObjectAttached(
                            IEnumerable<ulong> connections, uint objectIndex, byte map, ushort x, ushort y
                        )
                        {
                            return sender.Result.BroadcastObjectAttached(connections, id, objectIndex, map, x, y);
                        }

                        // Sends ObjectDetached to all the connections.
                        private Task BroadcastObjectDetached(
                            IEnumerable<ulong> connections, uint objectIndex
                        )
                        {
                            return sender.Result.BroadcastObjectDetached(connections, id, objectIndex);
                        }

                        // Broadcasts an ObjectTeleported message to all the connections.
                        private Task BroadcastObjectTeleported(
                            IEnumerable<ulong> connections, uint objectIndex, ushort x, ushort y
                        )
                        {
                            return sender.Result.BroadcastObjectTeleported(connections, id, objectIndex, x, y);
                        }

                        // Broadcasts an ObjectSpeedChanged message to all the connections.
                        private Task BroadcastSpeedChanged(
                            IEnumerable<ulong> connections, uint objectIndex, uint speed
                        )
                        {
                            return sender.Result.BroadcastSpeedChanged(connections, id, objectIndex, speed);
                        }

                        // Broadcasts an ObjectOrientationChanged message to all the connections.
                        private Task BroadcastOrientationChanged(
                            IEnumerable<ulong> connections, uint objectIndex, Direction orientation
                        )
                        {
                            return sender.Result.BroadcastOrientationChanged(connections, id, objectIndex, orientation);
                        }

                        // Broadcasts an ObjectMovementStarted message to all the connections.
                        private Task BroadcastMovementStarted(
                            IEnumerable<ulong> connections, uint objectIndex, ushort startX, ushort startY, Direction movement
                        )
                        {
                            return sender.Result.BroadcastMovementStarted(connections, id, objectIndex, startX, startY, movement);
                        }

                        // Broadcasts an ObjectMovementFinished message to all the connections.
                        private Task BroadcastMovementFinished(
                            IEnumerable<ulong> connections, uint objectIndex, ushort endX, ushort endY
                        )
                        {
                            return sender.Result.BroadcastMovementFinished(connections, id, objectIndex, endX, endY);
                        }

                        // Broadcasts an ObjectMovementCancelled message to all the connections.
                        private Task BroadcastMovementCancelled(
                            IEnumerable<ulong> connections, uint objectIndex, ushort revertToX, ushort revertToY
                        )
                        {
                            return sender.Result.BroadcastMovementCancelled(connections, id, objectIndex, revertToX, revertToY);
                        }
                    }
                }
            }
        }
    }
}
