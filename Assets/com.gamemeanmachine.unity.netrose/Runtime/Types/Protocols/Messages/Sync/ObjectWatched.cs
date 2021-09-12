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
            /// <typeparam name="T">The type of the watched model's data - matching for the given ContextId</typeparam>
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
                ///   The id/index of the context this object is
                ///   being watched. Objects have a default context
                ///   (which cannot be "watched"/"unwatched") and
                ///   can have more contexts to be watched. In the
                ///   server side, those contexts exist as behaviours,
                ///   while in the client side, external UI behaviours
                ///   can be set to watch a limited subset of these
                ///   objects in certain contexts (e.g. only one of
                ///   object is watched at the same time, in client
                ///   side, regarding health/status: the owned one;
                ///   only two objects at the same time will be watched
                ///   regarding the inventory: the owned one and an
                ///   extra one;...). The value in this property is
                ///   the 0-based index of the behaviour being watched.
                ///   This index will refer a watched model of type T.
                /// </summary>
                public ushort ContextId;

                /// <summary>
                ///   The object's data.
                /// </summary>
                public T Data;

                public void Serialize(Serializer serializer)
                {
                    serializer.Serialize(ref ScopeInstanceIndex);
                    serializer.Serialize(ref ObjectInstanceIndex);
                    serializer.Serialize(ref ContextId);
                    Data = Data ?? new T();
                    Data.Serialize(serializer);
                }
            }
        }
    }
}
