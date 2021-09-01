using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMeanMachine.Unity.NetRose
{
    namespace Authoring
    {
        namespace Behaviours
        {
            namespace World
            {
                /// <summary>
                ///   A networked map is also tied to specific game
                ///   server(s) and also can track itself in what
                ///   will be deemed as "scope".
                /// </summary>
                [RequireComponent(typeof(Map))]
                public class NetworkedMap : MonoBehaviour
                {
                    // TODO implement and define this one better.
                }
            }
        }
    }
}
