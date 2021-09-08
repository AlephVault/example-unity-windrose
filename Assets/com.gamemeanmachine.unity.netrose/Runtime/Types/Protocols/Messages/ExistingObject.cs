namespace GameMeanMachine.Unity.NetRose
{
    namespace Types
    {
        namespace Protocols
        {
            using AlephVault.Unity.Binary;

            /// <summary>
            ///   This message tells the client about an existing
            ///   object in a scope they belong to. The new object
            ///   will synchronize: scope index, object's  prefab
            ///   index, object's index, object's data, and whether
            ///   the object is attached to a map, and which map
            ///   and position, if true.
            /// </summary>
            public class ExistingObject<T> : ISerializable where T : ISerializable, new()
            {
                /// <summary>
                ///   The server-side index of the scope the client
                ///   is connected to, and the object belongs to.
                /// </summary>
                public uint ScopeInstanceIndex;

                /// <summary>
                ///   The index of the prefab the object is made with.
                /// </summary>
                public uint ObjectPrefabIndex;

                /// <summary>
                ///   The server-side index of the object being
                ///   spawned into the client.
                /// </summary>
                public uint ObjectInstanceIndex;

                /// <summary>
                ///   The existing object's data.
                /// </summary>
                public T Data;

                /// <summary>
                ///   Whether the object is attached to a map.
                /// </summary>
                public bool Attached;

                /// <summary>
                ///   The index of the map, if the object is attached.
                /// </summary>
                public byte MapIndex;

                /// <summary>
                ///   The X position, if the object is attached.
                /// </summary>
                public ushort X;

                /// <summary>
                ///   The Y position, if the object is attached.
                /// </summary>
                public ushort Y;

                public void Serialize(Serializer serializer)
                {
                    serializer.Serialize(ref ScopeInstanceIndex);
                    serializer.Serialize(ref ObjectPrefabIndex);
                    serializer.Serialize(ref ObjectInstanceIndex);
                    Data = Data ?? new T();
                    Data.Serialize(serializer);
                    serializer.Serialize(ref Attached);
                    if (Attached)
                    {
                        serializer.Serialize(ref MapIndex);
                        serializer.Serialize(ref X);
                        serializer.Serialize(ref Y);
                    }
                }
            }
        }
    }
}
