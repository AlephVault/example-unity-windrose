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
                ///     This message tells the client that an object
                ///     is being detached from its map, inside a
                ///     scope the client is watching.
                ///   </para>
                ///   <para>
                ///     It is only received from already connected
                ///     clients and only from existing objects, since
                ///     the first synchronization of an object will
                ///     have the current map information already in
                ///     the message, if attached.
                ///   </para>
                /// </summary>
                public class ObjectDetached : ISerializable
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
