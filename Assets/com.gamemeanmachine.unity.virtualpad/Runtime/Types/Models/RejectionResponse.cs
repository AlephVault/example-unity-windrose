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
            ///   A rejection message may involve one
            ///   or more fields, and it only applies
            ///   to controls (since applications do
            ///   not interact with the game, but only
            ///   listen).
            /// </summary>
            public class RejectionResponse : ISerializable
            {
                /// <summary>
                ///   The reason this control was rejected because.
                /// </summary>
                public RejectReason Reason;
                
                /// <summary>
                ///   The label being used. It is an
                ///   error to use an empty label, and
                ///   it is a cause for rejection.
                /// </summary>
                public string Label;
                
                /// <summary>
                ///   The index to use. Either 0 (to choose one
                ///   automatically) or 1-8 (to choose a specific
                ///   place automatically). If an index was chosen,
                ///   then an error might involve it being in use.
                /// </summary>
                public byte Index;
                
                /// <summary>
                ///   The password. If the server requires the user
                ///   to provide a password, it is an error that
                ///   the passwords do not match.
                /// </summary>
                public string Password;

                public void Serialize(Serializer serializer)
                {
                    serializer.Serialize(ref Label);
                    serializer.Serialize(ref Password);
                    serializer.Serialize(ref Index);
                }
            }
        }
    }
}
