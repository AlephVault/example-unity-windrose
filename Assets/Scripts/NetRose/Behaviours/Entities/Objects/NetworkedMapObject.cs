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

                    /// <summary>
                    ///   Invoked when this object is activated on the network in the client side,
                    ///     starts the queue processing.
                    /// </summary>
                    public override void OnStartClient()
                    {
                        // TODO prepare queue locally.
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
                            // TODO implement.
                        }
                    }

                    [ClientRpc]
                    private void RpcOnTeleported(uint x, uint y)
                    {
                        if (!isServer)
                        {
                            // TODO implement.
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
                            // TODO implement.
                        }
                    }

                    [ClientRpc]
                    private void RpcOnStateKeyChanged(string stateKey)
                    {
                        if (!isServer)
                        {
                            // TODO implement.
                        }
                    }

                    [ClientRpc]
                    private void RpcOnOrientationChanged(Direction orientation)
                    {
                        if (!isServer)
                        {
                            // TODO implement.
                        }
                    }

                    [ClientRpc]
                    private void RpcOnDetached()
                    {
                        if (!isServer)
                        {
                            // TODO implement.
                        }
                    }

                    /**
                     * Los mensajes recibidos tienen diferentes funciones:
                     * 
                     * MovementStart:
                     *   i. Tienen un
                     * 
                     *   1. Esperamos a que se quede quieto. Esto significa: esperamos
                     *      a que los movimientos (MovementStart) anteriores de la cola
                     *      se terminen/cancelen (esto vale por el hecho de que este
                     *      debería ser el único medio por el que se mueva el objeto).
                     *   2. Iniciar el movimiento (MovementStart).
                     *   3. Lanza el procedimiento asíncrono (async void) de esta forma:
                     *      i. Mientras existe el movimiento iniciado recientemente
                     *         (o sea: Movement != null), y esta acción no est
                     * MovementFinish:
                     *   En realidad no hace nada, pero sirve de "marcador" para
                     *   cuando "aceleremos" el movimiento de la cola de comandos.
                     */
                }
            }
        }
    }
}
