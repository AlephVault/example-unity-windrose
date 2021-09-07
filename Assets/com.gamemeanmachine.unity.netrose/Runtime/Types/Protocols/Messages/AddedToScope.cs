namespace GameMeanMachine.Unity.NetRose
{
    namespace Types
    {
        namespace Protocols
        {
            using AlephVault.Unity.Binary;

            /// <summary>
            ///   This message is sent to the client to tell that it was
            ///   added to a specific NEW scope. The client is NOT already
            ///   added to this scope (given by its instance index). Both
            ///   the prefab and the server-side index of this new scope
            ///   are given in this message, so the client can load it
            ///   and/or dispose it appropriately.
            /// </summary>
            public class AddedToScope : ISerializable
            {
                /// <summary>
                ///   The index of the prefab of the scope the client
                ///   was added to. This scope must be instantiated.
                /// </summary>
                public uint ScopePrefabIndex;

                /// <summary>
                ///   The server-side index of the scope the client
                ///   was added to.
                /// </summary>
                public uint ScopeInstanceIndex;

                public void Serialize(Serializer serializer)
                {
                    serializer.Serialize(ref ScopePrefabIndex);
                    serializer.Serialize(ref ScopeInstanceIndex);
                }
            }
        }
    }
}
