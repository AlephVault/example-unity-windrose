namespace GameMeanMachine.Unity.NetRose
{
    namespace Types
    {
        namespace Protocols
        {
            using AlephVault.Unity.Binary;

            /// <summary>
            ///   <para>
            ///     This message tells the client that, for certain
            ///     object, its whole data has been told to refresh.
            ///     This data may be related to a primary model or
            ///     a watched one (provided it is already watching
            ///     that object, in that particular watched model).
            ///   </para>
            /// </summary>
            /// <typeparam name="T">The type of the primary/watched model's data</typeparam>
            public class ObjectRefresh<T> : ISerializable where T : ISerializable, new()
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
