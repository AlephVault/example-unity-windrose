namespace GameMeanMachine.Unity.NetRose
{
    namespace Types
    {
        namespace Protocols
        {
            using AlephVault.Unity.Binary;

            /// <summary>
            ///   <para>
            ///     This message tells the client that an object
            ///     is being changed the value in one of its
            ///     properties. For scalar properties, typically
            ///     the provided value is a scalar. For collection
            ///     properties, the provided value is most of the
            ///     times some sort of "delta", to be processed
            ///     purely on arrival, in combination with the
            ///     current value in that property.
            ///   </para>
            ///   <para>
            ///     Each property is obtained by mapping the full
            ///     path (namespace.class.property) into a number,
            ///     and that number is mapped back into the property
            ///     on arrival (special care is taken to preserve
            ///     the string references).
            ///   </para>
            /// </summary>
            public class ObjectPropertyUpdated<T> : ISerializable where T : ISerializable, new()
            {
                /// <summary>
                ///   The server-side index of the scope the involved
                ///   object belongs to.
                /// </summary>
                public uint ScopeInstanceIndex;

                /// <summary>
                ///   The server-side index of the involved object.
                /// </summary>
                public uint ObjectInstanceIndex;

                /// <summary>
                ///   The index of the property being updated.
                /// </summary>
                public ushort PropertyIndex;

                /// <summary>
                ///   The value of the property being updated.
                ///   Either a full value, or a delta.
                /// </summary>
                public T Value;

                public void Serialize(Serializer serializer)
                {
                    serializer.Serialize(ref ScopeInstanceIndex);
                    serializer.Serialize(ref ObjectInstanceIndex);
                    serializer.Serialize(ref PropertyIndex);
                    Value = Value ?? new T();
                    Value.Serialize(serializer);
                }
            }
        }
    }
}
