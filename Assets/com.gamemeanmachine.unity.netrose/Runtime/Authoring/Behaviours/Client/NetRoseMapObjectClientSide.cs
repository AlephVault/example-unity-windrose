using AlephVault.Unity.Meetgard.Scopes.Authoring.Behaviours.Client;
using GameMeanMachine.Unity.NetRose.Types.Models;
using GameMeanMachine.Unity.NetRose.Types.Protocols.Messages;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Objects;
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
                public class NetRoseMapObjectClientSide : MonoBehaviour
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

                    // Processes an attachment event.
                    internal void OnAttached(Attachment attachment)
                    {
                        // TODO implement.
                    }

                    // Processes a detachment event.
                    internal void OnDetached()
                    {
                        // TODO implement.
                    }

                    // Processes a movement start event.
                    internal void OnMovementStarted(MovementStart movementStart)
                    {
                        // TODO implement.
                    }

                    // Processes a movement cancel event.
                    internal void OnMovementCancelled(Position position)
                    {
                        // TODO implement.
                    }

                    // Processes a movement finish event.
                    internal void OnMovementFinished(Position position)
                    {
                        // TODO implement.
                    }

                    // Processes a teleport event.
                    internal void OnTeleported(Position position)
                    {
                        // TODO implement.
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
