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
                ///     This message tells the client to despawn
                ///     an existing object, entirely (this also
                ///     involves stopping any additional watch
                ///     on it, like inventory or health/status).
                ///   </para>
                ///   <para>
                ///     There are two moments this message may be
                ///     got for a given object: Either the object
                ///     was destroyed in the server side, or the
                ///     client is being disconnected from the
                ///     scope the object belongs to.
                ///   </para>
                /// </summary>
                public class ObjectDespawned : ISerializable
                {
                    /// <summary>
                    ///   The server-side index of the scope the
                    ///   client is connected to, and the object
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
}
