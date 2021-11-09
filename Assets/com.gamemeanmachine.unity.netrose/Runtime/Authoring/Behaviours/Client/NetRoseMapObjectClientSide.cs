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

                    // Processes an attachment event.
                    internal void OnAttached(Map map, ushort x, ushort y)
                    {
                        ClearQueue();
                        MapObject.CancelMovement();
                        MapObject.Attach(map, x, y, true);
                    }

                    // Processes a detachment event.
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

                    // Processes a teleport event.
                    internal void OnTeleported(ushort x, ushort y)
                    {
                        ClearQueue();
                        MapObject.Teleport(x, y);
                        MapObject.Detach();

                    }

                    // Processes a speed change event.
                    internal void OnSpeedChanged(uint speed)
                    {
                        // TODO implement.
                    }

                    // Processes an orientation change event.
                    internal void OnOrientationChanged(Direction orientation)
                    {
                        // TODO implement.
                    }
                }
            }
        }
    }
}
