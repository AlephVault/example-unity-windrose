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
                ///     object, it started watching it in particular
                ///     context, received an initial synchronization of
                ///     its data, and will start receiving more updates
                ///     from it until it is despawned or the client stops
                ///     watching the object in that context.
                ///   </para>
                ///   <para>
                ///     Typically, "owned" objects will start being
                ///     watched in all of the available contexts, while
                ///     non-owned objects will eventually start being
                ///     watched and then unwatched appropriately on need.
                ///   </para>
                /// </summary>
                /// <typeparam name="T">The type of the watched model's data</typeparam>
                public class ObjectWatched<T> : ISerializable where T : ISerializable, new()
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
                    public T Data;

                    public void Serialize(Serializer serializer)
                    {
                        serializer.Serialize(ref ScopeInstanceIndex);
                        serializer.Serialize(ref ObjectInstanceIndex);
                        Data = Data ?? new T();
                        Data.Serialize(serializer);
                    }
                }
            }
        }
    }
}
