namespace GameMeanMachine.Unity.NetRose
{
    namespace Types
    {
        namespace Protocols
        {
            using AlephVault.Unity.Binary;

            /// <summary>
            ///   This message tells the client that an object
            ///   is being detached from its map, inside a
            ///   scope the client is watching.
            /// </summary>
            public class ObjectDetached : ISerializable
            {
                /// <summary>
                ///   The server-side index of the scope the client
                ///   is added to, and the object belongs to.
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
