using AlephVault.Unity.Meetgard.Scopes.Authoring.Behaviours.Server;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Objects;
using UnityEngine;


namespace GameMeanMachine.Unity.NetRose
{
    namespace Authoring
    {
        namespace Behaviours
        {
            namespace Server
            {
                /// <summary>
                ///   Map objects exist in server side, reflecting what is changed
                ///   in them to the client side, and sending those changes to the
                ///   client side by means of the current scope. These ones are also
                ///   related to a single WindRose map object in a single map.
                /// </summary>
                [RequireComponent(typeof(ObjectServerSide))]
                [RequireComponent(typeof(MapObject))]
                public class NetRoseMapObjectServerSide : MonoBehaviour
                {
                    // TODO implement.
                }
            }
        }
    }
}
