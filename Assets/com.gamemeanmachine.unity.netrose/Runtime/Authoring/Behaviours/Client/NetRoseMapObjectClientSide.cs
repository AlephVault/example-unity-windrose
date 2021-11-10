using AlephVault.Unity.Meetgard.Scopes.Authoring.Behaviours.Client;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Objects;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.World;
using GameMeanMachine.Unity.WindRose.Types;
using UnityEngine;


namespace GameMeanMachine.Unity.NetRose
{
    namespace Authoring
    {
        namespace Behaviours
        {
            namespace Client
            {
                /// <summary>
                ///   Map objects exist in client side, reflecting what is received
                ///   from the server side, and applying some internal mechanisms
                ///   for lag balance and recovery. These ones are also related to
                ///   a single WindRose map object in a single map.
                /// </summary>
                [RequireComponent(typeof(ObjectClientSide))]
                [RequireComponent(typeof(MapObject))]
                public partial class NetRoseMapObjectClientSide : MonoBehaviour
                {
                    /// <summary>
                    ///   The related object client side.
                    /// </summary>
                    public ObjectClientSide ObjectClientSide { get; private set; }

                    /// <summary>
                    ///   The related WindRose map object.
                    /// </summary>
                    public MapObject MapObject { get; private set; }

                    // The lag tolerance, as retrieved from the protocol.
                    private ushort lagTolerance;

                    // Tells whether it is spawned or not.
                    private bool spawned = false;

                    private void Awake()
                    {
                        ObjectClientSide = GetComponent<ObjectClientSide>();
                        ObjectClientSide.OnSpawned += OnSpawned;
                        ObjectClientSide.OnDespawned += OnDespawned;
                        MapObject = GetComponent<MapObject>();
                        MapObject.onMovementFinished.AddListener(OnMovementFinished);
                        MapObject.onMovementCancelled.AddListener(OnMovementCancelled);
                    }

                    private void OnDestroy()
                    {
                        ObjectClientSide.OnSpawned -= OnSpawned;
                        ObjectClientSide.OnDespawned -= OnDespawned;
                        MapObject.onMovementFinished.RemoveListener(OnMovementFinished);
                        MapObject.onMovementCancelled.RemoveListener(OnMovementCancelled);
                    }

                    // On spawn, set the lag tolerance, and the spawned.
                    private void OnSpawned()
                    {
                        NetRoseProtocolClientSide protocol = ObjectClientSide.Protocol.GetComponent<NetRoseProtocolClientSide>();
                        lagTolerance = protocol != null ? protocol.LagTolerance : (ushort)5;
                        spawned = true;
                    }

                    // On despawn, clear the lag tolerance, the spawned flag, and the queue.
                    private void OnDespawned()
                    {
                        lagTolerance = 0;
                        spawned = false;
                        queue.Clear();
                    }

                    // On movement finished, continue executing the queue.
                    private void OnMovementFinished(Direction direction)
                    {
                        if (spawned) RunQueue(false);
                    }

                    // On movement ccancelled, continue executing the queue.
                    private void OnMovementCancelled(Direction? direction)
                    {
                        if (spawned) RunQueue(false);
                    }

                    //
                    // From this point, all the network-related events start.
                    //

                    // Processes an attached event. This is immediate:
                    // Clears the queue (if it is executing, it stops),
                    // cancels the current movement (if any) and does
                    // the attachment.
                    internal void OnAttached(Map map, ushort x, ushort y)
                    {
                        if (!spawned) return;
                        queue.Clear();
                        MapObject.Attach(map, x, y, true);
                    }

                    // Processes a detached event. This is immediate:
                    // Clears the queue (if it is executing, it stops),
                    // cancels the current movement (if any) and does
                    // the detachment.
                    internal void OnDetached()
                    {
                        if (!spawned) return;
                        queue.Clear();
                        MapObject.Detach();
                    }

                    // Processes a teleport event. This is immediate:
                    // Clears the queue (if it is executing, it stops),
                    // cancels the current movement (if any) and does
                    // the teleport.
                    internal void OnTeleported(ushort x, ushort y)
                    {
                        if (!spawned) return;
                        queue.Clear();
                        MapObject.Teleport(x, y);
                    }

                    // Processes a movement start event. It queues the
                    // MovementStart command and, if the queue is not
                    // currently executing, it is now executed.
                    internal void OnMovementStarted(ushort x, ushort y, Direction direction)
                    {
                        if (!spawned) return;
                        QueueElement(new MovementStartCommand() { StartX = x, StartY = y, Direction = direction });
                    }

                    // Processes a movement cancel event. It queues the
                    // MovementCancel command and, if the queue is not
                    // currently executing, it is now executed.
                    internal void OnMovementCancelled(ushort x, ushort y)
                    {
                        if (!spawned) return;
                        QueueElement(new MovementCancelCommand() { RevertX = x, RevertY = y });
                    }

                    // Processes a movement finish event. It queues the
                    // MovementFinish command and, if the queue is not
                    // currently executing, it is now executed.
                    internal void OnMovementFinished(ushort x, ushort y)
                    {
                        if (!spawned) return;
                        QueueElement(new MovementFinishCommand() { EndX = x, EndY = y });
                    }

                    // Processes a movement speed change event. It queues
                    // the SpeedChanged command and, if the queue is not
                    // currently executing, it is now executed.
                    internal void OnSpeedChanged(uint speed)
                    {
                        if (!spawned) return;
                        QueueElement(new SpeedChangeCommand() { Speed = speed });
                    }

                    // Processes an orientation change event. It queues the
                    // OrientationChanged command and, if the queue is not
                    // currently executing, it is now executed.
                    internal void OnOrientationChanged(Direction orientation)
                    {
                        if (!spawned) return;
                        QueueElement(new OrientationChangeCommand() { Orientation = orientation });
                    }
                }
            }
        }
    }
}
