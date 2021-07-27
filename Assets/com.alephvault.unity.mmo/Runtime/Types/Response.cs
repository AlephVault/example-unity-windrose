using MLAPI.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlephVault.Unity.MMO
{
    namespace Types
    {
        /// <summary>
        ///   <para>
        ///     Response objects are for generic purpose and
        ///     typically to tell about success or failure,
        ///     with a success (or failure) code, and some
        ///     details in the case of failure.
        ///   </para>
        ///   <para>
        ///     Since the details are string, this is not
        ///     intendded as a generic return value for
        ///     anything.
        ///   </para>
        /// </summary>
        public struct Response : INetworkSerializable
        {
            /// <summary>
            ///   Whether the operation was successful or not.
            /// </summary>
            public bool Success;

            /// <summary>
            ///   The result code.
            /// </summary>
            public string Code;

            /// <summary>
            ///   An arbitrary list of details, mostly intended
            ///   for failure messages.
            /// </summary>
            public Dictionary<string, string> Details;

            /// <summary>
            ///   Serializes the <code>Code</code> and <code>Success</code>
            ///   fields, and uses a custom serialization logic to
            ///   serialize the <code>Details</code> dictionary.
            /// </summary>
            /// <param name="serializer">The network serializer</param>
            public void NetworkSerialize(NetworkSerializer serializer)
            {
                serializer.Serialize(ref Success);
                serializer.Serialize(ref Code);
                SerializeDetails(serializer);
            }

            private void SerializeDetails(NetworkSerializer serializer)
            {
                if (serializer.IsReading)
                {
                    ReadDetails(serializer.Reader);
                }
                else
                {
                    WriteDetails(serializer.Writer);
                }
            }

            private void ReadDetails(NetworkReader reader)
            {
                Details = new Dictionary<string, string>();
                int count = reader.ReadInt32();
                for (int index = 0; index < count; index++)
                {
                    string key = reader.ReadString().ToString();
                    string value = reader.ReadString().ToString();
                    Details.Add(key, value);
                }
            }

            private void WriteDetails(NetworkWriter writer)
            {
                if (Details != null)
                {
                    writer.WriteInt32(Details.Count);
                    foreach (KeyValuePair<string, string> pair in Details)
                    {
                        writer.WriteString(pair.Key);
                        writer.WriteString(pair.Value);
                    }
                }
                else
                {
                    writer.WriteInt32(0);
                }
            }

            public override string ToString()
            {
                return string.Format("Result(Success={0}, Code={1})", Success, Code);
            }
        }
    }
}
