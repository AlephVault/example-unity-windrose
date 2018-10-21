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
            using World;

            /**
             * Not Just a marker class to ensure all Positionable objects
             *   are attached to an object of this class, when attached to
             *   the map - it also tells the underlying represented objects
             *   which sorting layer to use when rendering.
             */
            [RequireComponent(typeof(Tilemap))]
            public class ObjectsTilemap : MonoBehaviour
            {
                [SerializeField]
                private int sortingLayer;

                /**
                 * This sorting layer will be assigned to children objects.
                 */
                public int SortingLayer { get { return sortingLayer; } }

                void Start()
                {
                    Layout.RequireComponentInParent<Map>(this);                    
                }
            }
        }
    }
}
