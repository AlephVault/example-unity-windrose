namespace GameMeanMachine.Unity.NetRose
{
    namespace Types
    {
        namespace Protocols
        {
            using AlephVault.Unity.Binary;

            /// <summary>
            ///   This message tells the client that an object
            ///   is being despawned from a scope the client
            ///   belongs to (i.e. is connected right now).
            /// </summary>
            public class ObjectDespawned : ISerializable
            {
                /// <summary>
                ///   The server-side index of the scope the client
                ///   belongs to.
                /// </summary>
                public uint ScopeInstanceIndex;

                /// <summary>
                ///   The server-side index of the object being
                ///   despawned.
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
