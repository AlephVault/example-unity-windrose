using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Mirror;
using GMM.Utils;

namespace NetRose
{
    namespace Behaviours
    {
        namespace Entities
        {
            namespace Objects
            {
                using WindRose.Types;
                using WindRose.Behaviours.World;
                using WindRose.Behaviours.Entities.Objects;

                /// <summary>
                ///   In non-host connections, this object synchronizes via events all that happens
                ///     to the base MapObject behaviour. This accounts for all the events of such
                ///     behaviour, but not for the strategies (those will be synchronized by other
                ///     behaviours, typically one per strategy behaviour). In host behaviours, the
                ///     event callbacks will do nothing since everything already happened in server
                ///     side.
                /// </summary>
                [RequireComponent(typeof(MapObject))]
                public class NetworkedMapObject : BaseBehaviour
                {
                    /// <summary>
                    ///   Triggered when the new map to attach a networked map object, has no networked map behaviour.
                    /// </summary>
                    public class MapNotSynchronized : Exception
                    {
                        public MapNotSynchronized() { }
                        public MapNotSynchronized(string message) : base(message) { }
                        public MapNotSynchronized(string message, System.Exception inner) : base(message, inner) { }
                    }

                    // Command class that performs an attachment to a map.
                    private class AttachCommand : ClientRpcCommand
                    {
                        private MapObject mapObject;
                        private Map map;
                        private uint newX, newY;

                        public AttachCommand(MapObject target, Map to, uint x, uint y)
                        {
                            mapObject = target;
                            map = to;
                            newX = x;
                            newY = y;
                        }

                        public override async Task Invoke(Func<bool> mustAccelerate)
                        {
                            mapObject.Attach(map, newX, newY, true);
                        }
                    }

                    // Command class that performs a teleport on the object.
                    // Positions will be consistent, since this comes from
                    // server-side objects in the same condition.
                    private class TeleportCommand : ClientRpcCommand
                    {
                        private MapObject mapObject;
                        private uint newX, newY;

                        public TeleportCommand(MapObject target, uint x, uint y)
                        {
                            mapObject = target;
                            newX = x;
                            newY = y;
                        }

                        public override async Task Invoke(Func<bool> mustAccelerate)
                        {
                            // The teleportation will NOT be silent.
                            mapObject.Teleport(newX, newY);
                        }
                    }

                    // Command class that updates the speed of the object.
                    private class SpeedChangeCommand : ClientRpcCommand
                    {
                        private MapObject mapObject;
                        private uint newSpeed;

                        public SpeedChangeCommand(MapObject target, uint speed)
                        {
                            mapObject = target;
                            newSpeed = speed;
                        }

                        public override async Task Invoke(Func<bool> mustAccelerate)
                        {
                            mapObject.Speed = newSpeed;
                        }
                    }

                    // Command class that updates the orientation of the object.
                    private class OrientationChangeCommand : ClientRpcCommand
                    {
                        private MapObject mapObject;
                        private Direction newOrientation;

                        public OrientationChangeCommand(MapObject target, Direction orientation)
                        {
                            mapObject = target;
                            newOrientation = orientation;
                        }

                        public override async Task Invoke(Func<bool> mustAccelerate)
                        {
                            mapObject.Orientation = newOrientation;
                        }
                    }

                    // Command class that detaches the object.
                    private class DetachCommand : ClientRpcCommand
                    {
                        private MapObject mapObject;

                        public DetachCommand(MapObject target)
                        {
                            mapObject = target;
                        }

                        public override async Task Invoke(Func<bool> mustAccelerate)
                        {
                            mapObject.Detach();
                        }
                    }

                    private MapObject mapObject;

                    /// <summary>
                    ///   The limit of the movements queue for this object. At minimum, this value
                    ///     is <see cref="MIN_QUEUE_LIMIT" />, and when the movement queue passes
                    ///     this maximum size, all the movement actions will be forced, not waited.
                    /// </summary>
                    [SerializeField]
                    private uint queueLimit = MIN_QUEUE_LIMIT;

                    private const uint MIN_QUEUE_LIMIT = 3;

                    private void Awake()
                    {
                        queueLimit = (queueLimit < MIN_QUEUE_LIMIT) ? MIN_QUEUE_LIMIT : queueLimit;
                        mapObject = GetComponent<MapObject>();
                    }

                    private void Start()
                    {
                        mapObject.onAttached.AddListener(OnAttached);
                        mapObject.onTeleported.AddListener(RpcOnTeleported);
                        mapObject.onMovementStarted.AddListener(OnMovementStarted);
                        mapObject.onMovementFinished.AddListener(OnMovementFinished);
                        mapObject.onMovementCancelled.AddListener(OnMovementCancelled);
                        mapObject.onDetached.AddListener(RpcOnDetached);
                        mapObject.onSpeedChanged.AddListener(RpcOnSpeedChanged);
                        mapObject.onOrientationChanged.AddListener(RpcOnOrientationChanged);
                        // Notes: onPropertyUpdated will not be listened here, but in
                        //        object-strategy synchronizing components.
                    }

                    private void OnDestroy()
                    {
                        mapObject.onAttached.RemoveListener(OnAttached);
                        mapObject.onTeleported.RemoveListener(RpcOnTeleported);
                        mapObject.onMovementStarted.RemoveListener(OnMovementStarted);
                        mapObject.onMovementFinished.RemoveListener(OnMovementFinished);
                        mapObject.onMovementCancelled.RemoveListener(OnMovementCancelled);
                        mapObject.onDetached.RemoveListener(RpcOnDetached);
                        mapObject.onSpeedChanged.RemoveListener(RpcOnSpeedChanged);
                        mapObject.onOrientationChanged.RemoveListener(RpcOnOrientationChanged);
                        // Notes: onPropertyUpdated will not be removed here, but in
                        //        object-strategy synchronizing components.
                    }

                    private void OnAttached(Map map)
                    {
                        World.NetworkedMap networkedMap = map.GetComponent<World.NetworkedMap>();
                        if (networkedMap)
                        {
                            RpcOnAttached(map.GetComponent<NetworkIdentity>(), mapObject.X, mapObject.Y);
                        }
                        else
                        {
                            throw new MapNotSynchronized("Cannot synchronize OnAttached event because the map has no networked map behaviour");
                        }
                    }

                    [ClientRpc]
                    private void RpcOnAttached(NetworkIdentity map, uint x, uint y)
                    {
                        if (!isServer)
                        {
                            AddToQueue(new AttachCommand(mapObject, map.GetComponent<Map>(), x, y), true);
                        }
                    }

                    [ClientRpc]
                    private void RpcOnTeleported(uint x, uint y)
                    {
                        if (!isServer)
                        {
                            AddToQueue(new TeleportCommand(mapObject, x, y), true);
                        }
                    }

                    private void OnMovementStarted(Direction direction)
                    {
                        RpcOnMovementStarted(direction, mapObject.X, mapObject.Y);
                    }

                    private void OnMovementFinished(Direction direction)
                    {
                        RpcOnMovementFinished(direction, mapObject.X, mapObject.Y);
                    }

                    private void OnMovementCancelled(Direction? direction)
                    {
                        RpcOnMovementCancelled((direction.HasValue ? direction.Value : default(Direction)), direction.HasValue, mapObject.X, mapObject.Y);
                    }

                    [ClientRpc]
                    private void RpcOnMovementStarted(Direction direction, uint startX, uint startY)
                    {
                        if (!isServer)
                        {
                            // TODO implement.
                        }
                    }

                    [ClientRpc]
                    private void RpcOnMovementFinished(Direction direction, uint finalX, uint finalY)
                    {
                        if (!isServer)
                        {
                            // TODO Implement.
                        }
                    }

                    [ClientRpc]
                    private void RpcOnMovementCancelled(Direction direction, bool hasDirection, uint rollbackX, uint rollbackY)
                    {
                        if (!isServer)
                        {
                            // TODO implement.
                        }
                    }

                    [ClientRpc]
                    private void RpcOnSpeedChanged(uint speed)
                    {
                        if (!isServer)
                        {
                            AddToQueue(new SpeedChangeCommand(mapObject, speed));
                        }
                    }

                    [ClientRpc]
                    private void RpcOnOrientationChanged(Direction orientation)
                    {
                        if (!isServer)
                        {
                            AddToQueue(new OrientationChangeCommand(mapObject, orientation));
                        }
                    }

                    [ClientRpc]
                    private void RpcOnDetached()
                    {
                        if (!isServer)
                        {
                            AddToQueue(new DetachCommand(mapObject), true);
                        }
                    }
                }
            }
        }
    }
}
