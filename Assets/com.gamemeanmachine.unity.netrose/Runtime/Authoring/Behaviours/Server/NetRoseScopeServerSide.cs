using AlephVault.Unity.Meetgard.Scopes.Authoring.Behaviours.Server;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.World;
using System.Collections.Generic;
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
                ///   The related world scope (to get the maps).
                /// </summary>
                public Scope Maps { get; private set; }

                private void Awake()
                {
                    ScopeServerSide = GetComponent<ScopeServerSide>();
                    Maps = GetComponent<Scope>();
                }

                /// <summary>
                ///   The scope server side id.
                /// </summary>
                public uint Id { get { return ScopeServerSide.Id; } }

                // TODO when the protocol is created, implement a protocol method like this:
                // TODO ScopeServerSide.Protocol?.GetComponent<NetRoseProtocolServerSide>()

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
            }
        }
    }
}
