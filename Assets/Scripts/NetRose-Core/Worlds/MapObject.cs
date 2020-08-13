using UnityEngine;
using Mirror;

namespace NetRose
{
    namespace Worlds
    {
        using WindRose.Behaviours.World;
        using WindRose.Behaviours.Entities.Objects;

        [RequireComponent(typeof(MapObject))]
        public class NetworkedMapObject : NetworkBehaviour
        {
            private MapObject mapObject;
            private Movable movable;
            private Snapped snapped;
            private Oriented oriented;
            private StatePicker statePicker;

            private void Awake()
            {
                mapObject = GetComponent<MapObject>();
                movable = GetComponent<Movable>();
                snapped = GetComponent<Snapped>();
                oriented = GetComponent<Oriented>();
                statePicker = GetComponent<StatePicker>();    
            }

            private void Start()
            {
                mapObject.onAttached.AddListener(RpcOnAttached);
                mapObject.onTeleported.AddListener(RpcOnTeleported);
                mapObject.onMovementStarted.AddListener(OnMovementStarted);
                mapObject.onMovementFinished.AddListener(OnMovementFinished);
                mapObject.onMovementCancelled.AddListener(OnMovementCancelled);
                mapObject.onDetached.AddListener(RpcOnDetached);
            }

            private void OnDestroy()
            {
                mapObject.onAttached.RemoveListener(RpcOnAttached);
                mapObject.onTeleported.RemoveListener(RpcOnTeleported);
                mapObject.onMovementStarted.RemoveListener(OnMovementStarted);
                mapObject.onMovementFinished.RemoveListener(OnMovementFinished);
                mapObject.onMovementCancelled.RemoveListener(OnMovementCancelled);
                mapObject.onDetached.RemoveListener(RpcOnDetached);
            }

            /**
             * TODO implement this way: When a movement command is issued, it is with the current
             * or final position of the object.
             * 
             * When a movement is started on server, the client must queue a "start" of the local
             * object movement. When a movement is finished or cancelled on server, the client is
             * also notified. IF the finished or cancelled movement is N steps ahead of the current
             * player (an incremental rpc ID must be sent as well) then the object's position is
             * forced and the event is faked.
             */

            [ClientRpc]
            private void RpcOnAttached(Map map)
            {
                // TODO implement.
            }

            [ClientRpc]
            private void RpcOnTeleported(uint x, uint y)
            {
                // TODO implement.
            }

            private void OnMovementStarted(WindRose.Types.Direction direction)
            {
                RpcOnMovementStarted(direction, mapObject.X, mapObject.Y);
            }

            private void OnMovementFinished(WindRose.Types.Direction direction)
            {
                RpcOnMovementFinished(direction, mapObject.X, mapObject.Y);
            }

            private void OnMovementCancelled(WindRose.Types.Direction? direction)
            {
                RpcOnMovementCancelled(direction.HasValue ? direction.Value : default(WindRose.Types.Direction), direction.HasValue, mapObject.X, mapObject.Y);
            }

            [ClientRpc]
            private void RpcOnMovementStarted(WindRose.Types.Direction direction, uint startX, uint startY)
            {
                // TODO implement.
            }

            [ClientRpc]
            private void RpcOnMovementFinished(WindRose.Types.Direction direction, uint finalX, uint finalY)
            {
                // TODO implement.
            }

            [ClientRpc]
            private void RpcOnMovementCancelled(WindRose.Types.Direction direction, bool hasDirection, uint rollbackX, uint rollbackY)
            {
                // TODO implement.
            }

            [ClientRpc]
            private void RpcOnDetached()
            {
                // TODO implement.
            }
        }
    }
}
