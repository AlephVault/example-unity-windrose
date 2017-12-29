using UnityEngine;

namespace WindRose
{
    namespace Behaviors
    {
        /**
         * This behaviour implements nothing but serves as
         *   a marker to be used when iterating on the Map
         *   children game objects.
         * 
         * Children game objects with this behaviour will be
         *   sent a message to pause and resume, accordingly.
         */
        public class Pausable : MonoBehaviour
        {
        }
    }
}
