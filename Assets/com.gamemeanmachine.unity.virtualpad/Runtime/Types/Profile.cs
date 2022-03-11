using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameMeanMachine.Unity.VirtualPad
{
    namespace Types
    {
        /// <summary>
        ///   The kind of connection done to the virtual pad hub.
        ///   Only pads and applications (games which make use of
        ///   those pads) are supported.
        /// </summary>
        public enum Profile
        {
            Pad, Application
        }
    }
}
