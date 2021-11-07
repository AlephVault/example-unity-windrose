using AlephVault.Unity.Meetgard.Scopes.Authoring.Behaviours.Client;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.World;
using UnityEngine;


namespace GameMeanMachine.Unity.NetRose
{
    namespace Authoring
    {
        namespace Behaviours
        {
            [RequireComponent(typeof(ScopeClientSide))]
            [RequireComponent(typeof(Scope))]
            public class NetRoseScopeClientSide : MonoBehaviour
            {
                /// <summary>
                ///   The related client server side.
                /// </summary>
                public ScopeClientSide ScopeClientSide { get; private set; }

                /// <summary>
                ///   The related world scope (to get the maps).
                /// </summary>
                public Scope Maps { get; private set; }

                private void Awake()
                {
                    ScopeClientSide = GetComponent<ScopeClientSide>();
                    Maps = GetComponent<Scope>();
                }

                /// <summary>
                ///   The scope client side id.
                /// </summary>
                public uint Id { get { return ScopeClientSide.Id; } }

                // TODO when the protocol is created, implement a protocol method like this:
                // TODO ScopeClientSide.Protocol?.GetComponent<NetRoseProtocolClientSide>()
            }
        }
    }
}
