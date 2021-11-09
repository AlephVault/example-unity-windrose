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

                    private void Awake()
                    {
                        ObjectClientSide = GetComponent<ObjectClientSide>();
                        ObjectClientSide.OnSpawned += OnSpawned;
                        ObjectClientSide.OnDespawned += OnDespawned;
                        MapObject = GetComponent<MapObject>();
                        MapObject.onMovementFinished.AddListener(OnMovementFinished);
                    }

                    private void OnDestroy()
                    {
                        ObjectClientSide.OnSpawned -= OnSpawned;
                        ObjectClientSide.OnDespawned -= OnDespawned;
                        MapObject.onMovementFinished.RemoveListener(OnMovementFinished);
                    }

                    // On spawn, set the lag tolerance.
                    private void OnSpawned()
                    {
                        NetRoseProtocolClientSide protocol = ObjectClientSide.Protocol.GetComponent<NetRoseProtocolClientSide>();
                        lagTolerance = protocol != null ? protocol.LagTolerance : (ushort)5;
                    }

                    // On despawn, clear the lag tolerance.
                    private void OnDespawned()
                    {
                        lagTolerance = 0;
                    }

                    // On movement finished, clear the current movement.
                    private void OnMovementFinished(Direction direction)
                    {
                        // TODO implement.
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
                        ClearQueue();
                        MapObject.CancelMovement();
                        MapObject.Attach(map, x, y, true);
                    }

                    // Processes a detached event. This is immediate:
                    // Clears the queue (if it is executing, it stops),
                    // cancels the current movement (if any) and does
                    // the detachment.
                    internal void OnDetached()
                    {
                        ClearQueue();
                        MapObject.CancelMovement();
                        MapObject.Detach();
                    }

                    // Processes a movement start event.
                    internal void OnMovementStarted(ushort x, ushort y, Direction direction)
                    {
                        // TODO implement.
                    }

                    // Processes a movement cancel event.
                    internal void OnMovementCancelled(ushort x, ushort y)
                    {
                        // TODO implement.
                    }

                    // Processes a movement finish event.
                    internal void OnMovementFinished(ushort x, ushort y)
                    {
                        // TODO implement.
                    }

                    // Processes a teleport event. This is immediate:
                    // Clears the queue (if it is executing, it stops),
                    // cancels the current movement (if any) and does
                    // the teleport.
                    internal void OnTeleported(ushort x, ushort y)
                    {
                        ClearQueue();
                        MapObject.CancelMovement();
                        MapObject.Teleport(x, y);
                    }

                    // Processes a movement speed change event. It queues
                    // the SpeedChanged command and, if the queue is not
                    // currently executing, it is now executed.
                    internal void OnSpeedChanged(uint speed)
                    {
                        QueueElement(new SpeedChangeCommand() { MapObject = MapObject, Speed = speed });
                        RunQueue();
                    }

                    // Processes an orientation change event. It queues the
                    // OrientationChanged command and, if the queue is not
                    // currently executing, it is now executed.
                    internal void OnOrientationChanged(Direction orientation)
                    {
                        QueueElement(new OrientationChangeCommand() { MapObject = MapObject, Orientation = orientation });
                        RunQueue();
                    }
                }
            }
        }
    }
}
