namespace GameMeanMachine.Unity.NetRose
{
    namespace Types
    {
        namespace Protocols
        {
            using AlephVault.Unity.Binary;

            /// <summary>
            ///   This message is sent to the client to tell that it was
            ///   removed from a specific scope. The given index is a
            ///   server-side index from an existing scopes.
            /// </summary>
            public class RemovedFromScope : ISerializable
            {
                /// <summary>
                ///   The server-side index of the prefab of the scope
                ///   the client was added to.
                /// </summary>
                public uint ScopeInstanceIndex;

                public void Serialize(Serializer serializer)
                {
                    serializer.Serialize(ref ScopeInstanceIndex);
                }
            }
        }
    }
}
