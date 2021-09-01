using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameMeanMachine.Unity.NetRose
{
    namespace Authoring
    {
        namespace Behaviours
        {
            namespace Entities.Objects
            {
                /// <summary>
                ///   A networked map object has its existence tied to
                ///   particular game server(s) (through an appropritate
                ///   protocol server side), with the implications: 1.
                ///   to keep a network identity to synchronize itself,
                ///   and 2. to provide different ways of reflecting
                ///   itself onto the clients.
                /// </summary>
                [RequireComponent(typeof(MapObject))]
                public class NetworkedMapObject : MonoBehaviour
                {
                    // TODO implement this class!

                    private MapObject mapObject;

                    private void Awake()
                    {
                        mapObject = GetComponent<MapObject>();
                    }
                }
            }
        }
    }
}
