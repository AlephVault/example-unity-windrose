namespace GameMeanMachine.Unity.NetRose
{
    namespace Types
    {
        namespace Protocols
        {
            using AlephVault.Unity.Binary;

            /// <summary>
            ///   <para>
            ///     This message is sent to the client to tell that it was
            ///     removed from a specific scope.
            ///   </para>
            ///   <para>
            ///     From the server perspective, the client was removed
            ///     from a scope it belonged to. The client, however, may
            ///     want to keep the scope object as some sort of cache
            ///     instead of disposing it.
            ///   </para>
            /// </summary>
            public class RemovedFromScope : ISerializable
            {
                /// <summary>
                ///   The server side index/ID of the Scope.
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
