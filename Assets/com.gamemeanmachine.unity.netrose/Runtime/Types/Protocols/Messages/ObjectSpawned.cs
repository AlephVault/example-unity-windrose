namespace GameMeanMachine.Unity.NetRose
{
    namespace Types
    {
        namespace Protocols
        {
            using AlephVault.Unity.Binary;

            /// <summary>
            ///   This message tells the client that an object
            ///   is being spawned in a scope the client belongs
            ///   to (i.e. is connected right now).
            /// </summary>
            public class ObjectSpawned<T> : ISerializable where T : ISerializable, new()
            {
                /// <summary>
                ///   The server-side index of the scope the client
                ///   is connected to, and the object belongs to.
                /// </summary>
                public uint ScopeInstanceIndex;

                /// <summary>
                ///   The index of the prefab the object to spawn
                ///   belongs to.
                /// </summary>
                public uint ObjectPrefabIndex;

                /// <summary>
                ///   The server-side index of the object being
                ///   spawned into the client.
                /// </summary>
                public uint ObjectInstanceIndex;

                /// <summary>
                ///   The spawned object's data.
                /// </summary>
                public T Data;

                public void Serialize(Serializer serializer)
                {
                    serializer.Serialize(ref ScopeInstanceIndex);
                    serializer.Serialize(ref ObjectPrefabIndex);
                    serializer.Serialize(ref ObjectInstanceIndex);
                    Data = Data ?? new T();
                    Data.Serialize(serializer);
                }
            }
        }
    }
}
