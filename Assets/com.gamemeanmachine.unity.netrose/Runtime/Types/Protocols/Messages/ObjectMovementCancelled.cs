namespace GameMeanMachine.Unity.NetRose
{
    namespace Types
    {
        namespace Protocols
        {
            using AlephVault.Unity.Binary;

            /// <summary>
            ///   This message tells the client that an object
            ///   cancelled its current movement.
            /// </summary>
            public class ObjectMovementCancelled : ISerializable
            {
                /// <summary>
                ///   The server-side index of the scope the client
                ///   belongs to.
                /// </summary>
                public uint ScopeInstanceIndex;

                /// <summary>
                ///   The server-side index of the involved object.
                /// </summary>
                public uint ObjectInstanceIndex;

                public void Serialize(Serializer serializer)
                {
                    serializer.Serialize(ref ScopeInstanceIndex);
                    serializer.Serialize(ref ObjectInstanceIndex);
                }
            }
        }
    }
}
