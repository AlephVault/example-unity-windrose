using AlephVault.Unity.Binary;


namespace AlephVault.Unity.Meetgard.Scopes
{
    namespace Types
    {
        namespace Protocols
        {
            namespace Messages
            {
                /// <summary>
                ///   A message telling an object in the current scope
                ///   is being spwaned. The client must spawn a new
                ///   object according to its prefab, the given data
                ///   (this implies: each object type has its own way
                ///   for data encoding/decoding regarding refresh).
                ///   Note: Is it not forcefully needed that the data
                ///   type used in this message matches the data type
                ///   used in the <see cref="ObjectRefreshed"/> messsage
                ///   for a given object type.
                /// </summary>
                public class ObjectSpawned : ISerializable
                {
                    /// <summary>
                    ///   The effective index of the current scope, sent
                    ///   for validity. This message should be discarded
                    ///   if the scope is no more valid.
                    /// </summary>
                    public uint ScopeIndex;

                    /// <summary>
                    ///   The index of the prefav this object is being spawned
                    ///   with (client and server must match the index in an
                    ///   appropriate way).
                    /// </summary>
                    public uint ObjectPrefabIndex;

                    /// <summary>
                    ///   The effective index of the object being spawned
                    ///   in the current scope.
                    /// </summary>
                    public uint ObjectIndex;

                    /// <summary>
                    ///   The full data to apply. How the data is received,
                    ///   decoded and interpreted is up to the client. What
                    ///   is known: this data is, actually, the dumped contents
                    ///   of an object satisfying <see cref="ISerializable"/>.
                    ///   Both the client and server must agree the underlying
                    ///   type being used to encode/decode the data.
                    /// </summary>
                    public byte[] Data;

                    public void Serialize(Serializer serializer)
                    {
                        serializer.Serialize(ref ScopeIndex);
                        serializer.Serialize(ref ObjectPrefabIndex);
                        serializer.Serialize(ref ObjectIndex);
                        serializer.Serialize(ref Data);
                    }
                }
            }
        }
    }
}