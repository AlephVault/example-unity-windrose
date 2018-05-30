using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Support.Utils;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Tilemaps
        {
            /**
             * Just a marker class to ensure all Positionable objects are
             *   attached to an object of this class, when attached to the
             *   map.
             */
            [RequireComponent(typeof(Tilemap))]
            public class ObjectsTilemap : MonoBehaviour
            {
                void Start()
                {
                    Layout.RequireComponentInParent<Map>(this);                    
                }
            }
        }
    }
}
