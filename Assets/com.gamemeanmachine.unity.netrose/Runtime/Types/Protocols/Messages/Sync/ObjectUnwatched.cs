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
                ///     This message tells the client that right now
                ///     it will stop receiving updates from the object
                ///     in a particular context. Typically, this message
                ///     is received when stopping receiving inventory
                ///     updates (from chest-like objects) or other types
                ///     of close-look of an object (it will seldom to
                ///     never happen to owned objects).
                ///   </para>
                ///   <para>
                ///     This message will be received in three possible
                ///     moments: Two of them involving the object being
                ///     despawned (this message will be sent BEFORE the
                ///     "despawn" message, and once for each context),
                ///     and the third one involving the particular need
                ///     to stop watching one particular object in one
                ///     particular context.
                ///   </para>
                /// </summary>
                public class ObjectUnwatched : ISerializable
                {
                    /// <summary>
                    ///   The server-side index of the scope the
                    ///   client is connected to, and the object
                    ///   belongs to.
                    /// </summary>
                    public uint ScopeInstanceIndex;

                    /// <summary>
                    ///   The server-side index of the object being
                    ///   unwatched by the client.
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
