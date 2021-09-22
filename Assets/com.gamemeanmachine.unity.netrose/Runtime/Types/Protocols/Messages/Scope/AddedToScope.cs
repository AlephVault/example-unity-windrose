namespace GameMeanMachine.Unity.NetRose
{
    namespace Types
    {
        namespace Protocols
        {
            namespace Messages
            {
                using AlephVault.Unity.Binary;

                /// <summary>
                ///   <para>
                ///     This message is sent to the client to tell that it was
                ///     added to a specific new scope.
                ///   </para>
                ///   <para>
                ///     From the server perspective, the client is NOT already
                ///     added to this scope (given by its instance index), but
                ///     the client side might have a cached copy to make use
                ///     of instead of re-creating a new environment.
                ///   </para>
                /// </summary>
                public class AddedToScope : ISerializable
                {
                    /// <summary>
                    ///   The prefab index. It must be a valid index (at least
                    ///   before receiving this message) of the Scope prefab.
                    /// </summary>
                    public uint ScopePrefabIndex;

                    /// <summary>
                    ///   The server side index/ID of the Scope.
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
}
