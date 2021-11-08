using AlephVault.Unity.Meetgard.Scopes.Authoring.Behaviours.Server;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.World;
using GameMeanMachine.Unity.WindRose.Types;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


namespace GameMeanMachine.Unity.NetRose
{
    namespace Authoring
    {
        namespace Behaviours
        {
            [RequireComponent(typeof(ScopeServerSide))]
            [RequireComponent(typeof(Scope))]
            public class NetRoseScopeServerSide : MonoBehaviour
            {
                /// <summary>
                ///   The related scope server side (to get the connections and objects).
                /// </summary>
                public ScopeServerSide ScopeServerSide { get; private set; }

                /// <summary>
                ///   The protocol this NetRose scope is related to.
                /// </summary>
                public NetRoseProtocolServerSide NetRoseProtocolServerSide { get; private set; }

                /// <summary>
                ///   The related world scope (to get the maps).
                /// </summary>
                public Scope Maps { get; private set; }

                private void Awake()
                {
                    ScopeServerSide = GetComponent<ScopeServerSide>();
                    Maps = GetComponent<Scope>();
                }

                private void Start()
                {
                    ScopeServerSide.OnLoad += ScopeServerSide_OnLoad;
                }

                private void OnDestroy()
                {
                    ScopeServerSide.OnLoad -= ScopeServerSide_OnLoad;
                }

                private async Task ScopeServerSide_OnLoad()
                {
                    NetRoseProtocolServerSide = ScopeServerSide.Protocol.GetComponent<NetRoseProtocolServerSide>();
                }

                /// <summary>
                ///   The scope server side id.
                /// </summary>
                public uint Id { get { return ScopeServerSide.Id; } }

                /// <summary>
                ///   Returns an iterator of all the objects in the scope.
                /// </summary>
                /// <returns>The iterator</returns>
                public IEnumerable<ObjectServerSide> Objects()
                {
                    return ScopeServerSide.Objects();
                }

                /// <summary>
                ///   Returns an iterator of all the connections in the scope.
                /// </summary>
                /// <returns>The iterator</returns>
                public IEnumerable<ulong> Connections()
                {
                    return ScopeServerSide.Connections();
                }

                // Now, the internal protocol methods are to be defined here.
                // These are the same internal methods in the protocol server side,
                // but without specifying the connections.

                /// <summary>
                ///   Broadcasts a "object attached" message. This message is triggered when
                ///   an object is added to a map in certain scope.
                /// </summary>
                /// <param name="objectId">The id of the object</param>
                /// <param name="mapIndex">The index of the map, inside the scope, this object is being added to</param>
                /// <param name="x">The x position of the object in the new map</param>
                /// <param name="y">The y position of the object in the new map</param>
                internal Task BroadcastObjectAttached(uint objectId, uint mapIndex, ushort x, ushort y)
                {
                    return NetRoseProtocolServerSide.BroadcastObjectAttached(Connections(), ScopeServerSide.Id, objectId, mapIndex, x, y);
                }

                /// <summary>
                ///   Broadcasts a "object attached" message. This message is triggered when
                ///   an object is added to a map in certain scope.
                /// </summary>
                /// <param name="objectId">The id of the object</param>
                internal Task BroadcastObjectDetached(uint objectId)
                {
                    return NetRoseProtocolServerSide.BroadcastObjectDetached(Connections(), ScopeServerSide.Id, objectId);
                }

                /// <summary>
                ///   Broadcasts a "object movement started" message. This message is triggered when
                ///   an object started moving inside a map in the current scope.
                /// </summary>
                /// <param name="objectId">The id of the object</param>
                /// <param name="x">The starting x position of the object when starting movement</param>
                /// <param name="y">The starting y position of the object when starting movement</param>
                /// <param name="direction">The direction of the object when starting movement</param>
                internal Task BroadcastObjectMovementStarted(uint objectId, ushort x, ushort y, Direction direction)
                {
                    return NetRoseProtocolServerSide.BroadcastObjectMovementStarted(Connections(), ScopeServerSide.Id, objectId, x, y, direction);
                }

                /// <summary>
                ///   Broadcasts a "object movement cancelled" message. This message is triggered when
                ///   an object cancelled moving inside a map in certain scope.
                /// </summary>
                /// <param name="objectId">The id of the object</param>
                /// <param name="x">The to-revert x position of the object when cancelling movement</param>
                /// <param name="y">The to-revert y position of the object when cancelling movement</param>
                internal Task BroadcastObjectMovementCancelled(uint objectId, ushort x, ushort y)
                {
                    return NetRoseProtocolServerSide.BroadcastObjectMovementCancelled(Connections(), ScopeServerSide.Id, objectId, x, y);
                }

                /// <summary>
                ///   Broadcasts a "object movement finished" message. This message is triggered when
                ///   an object finished moving inside a map in certain scope.
                /// </summary>
                /// <param name="objectId">The id of the object</param>
                /// <param name="x">The end x position of the object when finishing movement</param>
                /// <param name="y">The end y position of the object when finishing movement</param>
                internal Task BroadcastObjectMovementFinished(uint objectId, ushort x, ushort y)
                {
                    return NetRoseProtocolServerSide.BroadcastObjectMovementFinished(Connections(), ScopeServerSide.Id, objectId, x, y);
                }

                /// <summary>
                ///   Broadcasts a "object movement teleported" message. This message is triggered when
                ///   an object teleported inside a map in certain scope.
                /// </summary>
                /// <param name="objectId">The id of the object</param>
                /// <param name="x">The end x position of the object when teleporting</param>
                /// <param name="y">The end y position of the object when teleporting</param>
                internal Task BroadcastObjectTeleported(uint objectId, ushort x, ushort y)
                {
                    return NetRoseProtocolServerSide.BroadcastObjectTeleported(Connections(), ScopeServerSide.Id, objectId, x, y);
                }

                /// <summary>
                ///   Broadcasts a "object speed changed" message. This message is triggered when
                ///   an object changed its speed inside a map in certain scope.
                /// </summary>
                /// <param name="objectId">The id of the object</param>
                /// <param name="speed">The new object speed</param>
                internal Task BroadcastObjectSpeedChanged(uint objectId, uint speed)
                {
                    return NetRoseProtocolServerSide.BroadcastObjectSpeedChanged(Connections(), ScopeServerSide.Id, objectId, speed);
                }

                /// <summary>
                ///   Broadcasts a "object orientation changed" message. This message is triggered when
                ///   an object changed its orientation inside a map in certain scope.
                /// </summary>
                /// <param name="objectId">The id of the object</param>
                /// <param name="orientation">The new object orientation</param>
                internal Task BroadcastObjectOrientationChanged(uint objectId, Direction orientation)
                {
                    return NetRoseProtocolServerSide.BroadcastObjectOrientationChanged(Connections(), ScopeServerSide.Id, objectId, orientation);
                }
            }
        }
    }
}
