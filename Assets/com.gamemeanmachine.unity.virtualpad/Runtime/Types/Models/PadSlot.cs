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
            ///   Applications receive this message when a control
            ///   pad is successfully synchronized or desynchronized.
            /// </summary>
            public class PadSlot : ISerializable
            {
                /// <summary>
                ///   The label being used by the control pad.
                /// </summary>
                public string Label;
                
                /// <summary>
                ///   The index being used by the control pad.
                ///   An effective 1-8 virtual port index.
                /// </summary>
                public byte Index;
                
                public void Serialize(Serializer serializer)
                {
                    serializer.Serialize(ref Label);
                    serializer.Serialize(ref Index);
                }
            }
        }
    }
}
