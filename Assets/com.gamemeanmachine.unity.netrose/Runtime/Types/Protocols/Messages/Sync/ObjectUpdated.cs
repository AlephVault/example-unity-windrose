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
                ///     This message tells the client that, for certain
                ///     object, a particular property is being updated.
                ///   </para>
                ///   <para>
                ///     This message is received in two different moments:
                ///     One involves a "public" model property being
                ///     updated (which are seen by any object added to
                ///     the same scope the object belongs to), and the
                ///     other invovles a "watched" model property being
                ///     updated (which are seen by any connection currently
                ///     watching the object in a particular context).
                ///   </para>
                /// </summary>
                /// <typeparam name="T">The type of the watched model property's value</typeparam>
                public class ObjectUpdated<T> : ISerializable where T : ISerializable, new()
                {
                    /// <summary>
                    ///   The server-side index of the scope the client
                    ///   is connected to, and the object belongs to.
                    /// </summary>
                    public uint ScopeInstanceIndex;

                    /// <summary>
                    ///   The server-side index of the object being
                    ///   watched by the client.
                    /// </summary>
                    public uint ObjectInstanceIndex;

                    /// <summary>
                    ///   The object's data.
                    /// </summary>
                    public T Value;

                    public void Serialize(Serializer serializer)
                    {
                        serializer.Serialize(ref ScopeInstanceIndex);
                        serializer.Serialize(ref ObjectInstanceIndex);
                        Value = Value ?? new T();
                        Value.Serialize(serializer);
                    }
                }
            }
        }
    }
}
