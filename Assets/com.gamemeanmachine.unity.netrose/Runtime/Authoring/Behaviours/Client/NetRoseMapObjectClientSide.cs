using AlephVault.Unity.Meetgard.Scopes.Authoring.Behaviours.Client;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Objects;
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
                    // TODO implement.
                }
            }
        }
    }
}
