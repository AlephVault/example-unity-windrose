using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Serialization;

namespace AlephVault.Unity.MMO
{
    namespace Types
    {
        /// <summary>
        ///   A reason is typically a kind of asynchronous
        ///   response (i.e. not actually a response to
        ///   any request) stating whether it is a graceful
        ///   response or not, and details about.
        /// </summary>
        public struct Reason : INetworkSerializable
        {
            // A standard value to graceful logout.
            public static Reason LoggedOut = new Reason() { Graceful = true, Code = "__AV:MMO__:LOGGED-OUT", Text = "Logged Out" };

            /// <summary>
            ///   Whether the operation was graceful or not.
            /// </summary>
            public bool Graceful;

            /// <summary>
            ///   The result code.
            /// </summary>
            public string Code;

            /// <summary>
            ///    A more detailed reason text.
            /// </summary>
            public string Text;

            /// <summary>
            ///   Serializes the <code>Graceful</code>, <code>Code</code>
            ///   and <code>Text</code> fields in a standard way.
            /// </summary>
            /// <param name="serializer">The network serializer</param>
            public void NetworkSerialize(NetworkSerializer serializer)
            {
                serializer.Serialize(ref Graceful);
                serializer.Serialize(ref Code);
                serializer.Serialize(ref Text);
            }
        }
    }
}