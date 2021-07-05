using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Mirror;
using AlephVault.Unity.Support.Utils;

namespace NetRose
{
    namespace Behaviours
    {
        namespace Entities
        {
            namespace Objects
            {
                using GameMeanMachine.Unity.WindRose.Types;
                using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.World;
                using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Objects;
                using UnityEngine.Events;

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

                    // Command class that starts a movement for the object.
                    // The movement is started in certain direction, and this
                    // command is stored so it can told to finish, or can be
                    // forced to cancel. The logic will imply waiting for the
                    // current movement to end, then the command will start
                    // the movement, and an asynchronous task to check whether
                    // the movement should accelerate or not.
                    private class StartMovementCommand : ClientRpcCommand
                    {
                        private NetworkedMapObject owner;
                        private MapObject mapObject;
                        private uint startX, startY;
                        private Direction movement;

                        private bool willFinish = false;
                        private bool cancelled = false;
                        private bool currentMovementIsActive = false;

                        public StartMovementCommand(NetworkedMapObject me, MapObject target, Direction toward, uint x, uint y)
                        {
                            owner = me;
                            mapObject = target;
                            movement = toward;
                            startX = x;
                            startY = y;
                        }

                        /// <summary>
                        ///   Marks this start-movement command as the last one in the
                        ///     queue (being added sequentially).
                        /// </summary>
                        public override void OnEnqueued()
                        {
                            owner.lastStartMovementCommandEnqueued = this;
                        }

                        /// <summary>
                        ///   When dequeued, if this start-movement command was the one
                        ///     marked as "last" in the queue, such "last" will be cleared.
                        /// </summary>
                        public override void OnDequeued()
                        {
                            if (owner.lastStartMovementCommandEnqueued == this) owner.lastStartMovementCommandEnqueued = null;
                        }

                        /// <summary>
                        ///   This invocation waits until the movement is available,
                        ///     then attempts to start the movement (if not marked
                        ///     as cancelled), and while there, it will register
                        ///     event callbacks to clear its "single current movement"
                        ///     flag when finished/cancelled. Then, another async
                        ///     function will run, which will check whether the queue
                        ///     must be accelerated or not and, if the case, it will
                        ///     force the movement to finish. This, while the "single
                        ///     current movement" flag is active. After the movement
                        ///     ended or was cancelled, the callbacks are cleared.
                        /// </summary>
                        /// <param name="mustAccelerate">A function telling that the queue was marked to accelerate</param>
                        public override async Task Invoke(Func<bool> mustAccelerate)
                        {
                            while (mapObject.IsMoving) await Tasks.Blink();
                            // If the movement is cancelled (was cancelled beforehand)
                            // then do not even start the movement.
                            if (cancelled) return;
                            // If the coordinates do not match, force a SILENT teleport
                            // on the map object to the new coordinates.
                            if (mapObject.X != startX || mapObject.Y != startY) mapObject.Teleport(startX, startY, true);
                            // If the movement is not cancelled beforehand, it will
                            // attempt to start. The new movement will NOT be marked
                            // as continuated, but may be "queued" in the movement
                            // management itself to allow a smooth transition.
                            if (mapObject.StartMovement(movement, false, true))
                            {
                                currentMovementIsActive = true;
                                UnityAction<Direction> finished = delegate (Direction direction)
                                {
                                    currentMovementIsActive = false;
                                };
                                UnityAction<Direction?> cancelled = delegate (Direction? direction)
                                {
                                    currentMovementIsActive = false;
                                };
                                mapObject.onMovementFinished.AddListener(finished);
                                mapObject.onMovementCancelled.AddListener(cancelled);
                                TrackMovement(mustAccelerate, finished, cancelled);
                            }
                        }

                        // Tracks whether the movement should continue normally or accelerated.
                        private async void TrackMovement(Func<bool> mustAccelerate, UnityAction<Direction> finished, UnityAction<Direction?> cancelled)
                        {
                            while (currentMovementIsActive)
                            {
                                await Tasks.Blink();
                                if (mustAccelerate()) mapObject.FinishMovement();
                            }
                            // To this point, either the movement finished or it was cancelled.
                            // Said this, the event callbacks must be removed.
                            mapObject.onMovementFinished.RemoveListener(finished);
                            mapObject.onMovementCancelled.RemoveListener(cancelled);
                        }

                        /// <summary>
                        ///   Marks this command as: "it is guaranteed that this movement finishes".
                        /// </summary>
                        public void WillFinish()
                        {
                            if (!cancelled) willFinish = true;
                        }

                        /// <summary>
                        ///   Marks this command as potentially cancelled or even it actualy cancels it.
                        /// </summary>
                        public void Cancel()
                        {
                            if (!willFinish)
                            {
                                cancelled = true;
                                if (currentMovementIsActive) mapObject.CancelMovement();
                            }
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

                    // The underlying object being synchronized.
                    public MapObject MapObject { get; private set; }

                    // The LAST movement command being enqueued.
                    private StartMovementCommand lastStartMovementCommandEnqueued = null;

                    private void Awake()
                    {
                        MapObject = GetComponent<MapObject>();
                    }

                    private void Start()
                    {
                        MapObject.onAttached.AddListener(OnAttached);
                        MapObject.onTeleported.AddListener(RpcOnTeleported);
                        MapObject.onMovementStarted.AddListener(OnMovementStarted);
                        MapObject.onMovementFinished.AddListener(OnMovementFinished);
                        MapObject.onMovementCancelled.AddListener(OnMovementCancelled);
                        MapObject.onDetached.AddListener(RpcOnDetached);
                        MapObject.onSpeedChanged.AddListener(RpcOnSpeedChanged);
                        MapObject.onOrientationChanged.AddListener(RpcOnOrientationChanged);
                        // Notes: onPropertyUpdated will not be listened here, but in
                        //        object-strategy synchronizing components.
                    }

                    private void OnDestroy()
                    {
                        MapObject.onAttached.RemoveListener(OnAttached);
                        MapObject.onTeleported.RemoveListener(RpcOnTeleported);
                        MapObject.onMovementStarted.RemoveListener(OnMovementStarted);
                        MapObject.onMovementFinished.RemoveListener(OnMovementFinished);
                        MapObject.onMovementCancelled.RemoveListener(OnMovementCancelled);
                        MapObject.onDetached.RemoveListener(RpcOnDetached);
                        MapObject.onSpeedChanged.RemoveListener(RpcOnSpeedChanged);
                        MapObject.onOrientationChanged.RemoveListener(RpcOnOrientationChanged);
                        // Notes: onPropertyUpdated will not be removed here, but in
                        //        object-strategy synchronizing components.
                    }

                    private void OnAttached(Map map)
                    {
                        World.NetworkedMap networkedMap = map.GetComponent<World.NetworkedMap>();
                        if (networkedMap)
                        {
                            RpcOnAttached(map.GetComponent<NetworkIdentity>(), MapObject.X, MapObject.Y);
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
                            AddToQueue(new AttachCommand(MapObject, map.GetComponent<Map>(), x, y), true);
                        }
                    }

                    [ClientRpc]
                    private void RpcOnTeleported(uint x, uint y)
                    {
                        if (!isServer)
                        {
                            AddToQueue(new TeleportCommand(MapObject, x, y), true);
                        }
                    }

                    private void OnMovementStarted(Direction direction)
                    {
                        RpcOnMovementStarted(direction, MapObject.X, MapObject.Y);
                    }

                    private void OnMovementFinished(Direction direction)
                    {
                        RpcOnMovementFinished(direction, MapObject.X, MapObject.Y);
                    }

                    private void OnMovementCancelled(Direction? direction)
                    {
                        RpcOnMovementCancelled((direction.HasValue ? direction.Value : default(Direction)), direction.HasValue, MapObject.X, MapObject.Y);
                    }

                    [ClientRpc]
                    private void RpcOnMovementStarted(Direction direction, uint startX, uint startY)
                    {
                        if (!isServer)
                        {
                            AddToQueue(new StartMovementCommand(this, MapObject, direction, startX, startY));
                        }
                    }

                    [ClientRpc]
                    private void RpcOnMovementFinished(Direction direction, uint finalX, uint finalY)
                    {
                        if (!isServer)
                        {
                            if (lastStartMovementCommandEnqueued != null) lastStartMovementCommandEnqueued.WillFinish();
                        }
                    }

                    [ClientRpc]
                    private void RpcOnMovementCancelled(Direction direction, bool hasDirection, uint rollbackX, uint rollbackY)
                    {
                        if (!isServer)
                        {
                            if (lastStartMovementCommandEnqueued != null) lastStartMovementCommandEnqueued.Cancel();
                        }
                    }

                    [ClientRpc]
                    private void RpcOnSpeedChanged(uint speed)
                    {
                        if (!isServer)
                        {
                            AddToQueue(new SpeedChangeCommand(MapObject, speed));
                        }
                    }

                    [ClientRpc]
                    private void RpcOnOrientationChanged(Direction orientation)
                    {
                        if (!isServer)
                        {
                            AddToQueue(new OrientationChangeCommand(MapObject, orientation));
                        }
                    }

                    [ClientRpc]
                    private void RpcOnDetached()
                    {
                        if (!isServer)
                        {
                            AddToQueue(new DetachCommand(MapObject), true);
                        }
                    }
                }
            }
        }
    }
}
