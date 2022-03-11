using System.Collections;
using System.Collections.Generic;
using AlephVault.Unity.Binary;
using UnityEditor;
using UnityEngine;


namespace GameMeanMachine.Unity.VirtualPad
{
    namespace Types
    {
        namespace Models
        {
            /// <summary>
            ///   Joysticks have input events which change
            ///   the state of a given button or axis.
            /// </summary>
            public class VirtualJoystickInputState : ISerializable
            {
                /// <summary>
                ///   The key or axis that is being changed.
                /// </summary>
                public VirtualJoystickInput Input;

                /// <summary>
                ///   The new value for that key or axis.
                ///   0 means unpressed or neutral. This
                ///   value will be in [-1, 1] or a subset
                ///   of those values.
                /// </summary>
                public float Value;
                
                public void Serialize(Serializer serializer)
                {
                    serializer.Serialize(ref Input);
                    serializer.Serialize(ref Value);
                }
            }
        }
    }
}
