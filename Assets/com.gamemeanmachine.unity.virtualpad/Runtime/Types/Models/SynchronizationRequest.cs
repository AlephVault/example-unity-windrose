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
            ///   This is not just a greeting message, but also
            ///   a configuration message. Applications that want
            ///   to connect and receive controls interactions
            ///   will use the <see cref="Profile.Application" />
            ///   profile and provide a non-empty label. Players
            ///   that want to connect as controls must use the
            ///   <see cref="Profile.Pad" /> profile instead, and
            ///   also provide a label and optionally the index
            ///   of position to use, and perhaps a password (if
            ///   required by the server).
            /// </summary>
            public class SynchronizationRequest : ISerializable
            {
                /// <summary>
                ///   The profile to use.
                /// </summary>
                public Profile UsingProfile;
                
                /// <summary>
                ///   The label to use.
                /// </summary>
                public string Label;
                
                /// <summary>
                ///   The index to use. Either 0 (to choose one
                ///   automatically) or 1-8 (to choose a specific
                ///   place automatically).
                /// </summary>
                public byte Index = 0;
                
                /// <summary>
                ///   The password. This is only considered if
                ///   the requires a non-empty password. The
                ///   password must match, or the connection
                ///   will be rejected.
                /// </summary>
                public string Password = "";

                public void Serialize(Serializer serializer)
                {
                    if (!serializer.IsReading)
                    {
                        Label = Label?.Trim();
                        Password = Password?.Trim();
                    }
                    serializer.Serialize(ref UsingProfile);
                    serializer.Serialize(ref Label);
                    serializer.Serialize(ref Password);
                    serializer.Serialize(ref Index);
                    if (serializer.IsReading)
                    {
                        Label = Label?.Trim();
                        Password = Password?.Trim();
                    }
                }
            }
        }
    }
}
