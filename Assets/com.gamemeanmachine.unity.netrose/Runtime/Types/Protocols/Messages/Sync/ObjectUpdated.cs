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
            /// <typeparam name="T">The type of the watched model property's value - matching for the given ContextId</typeparam>
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
                ///   This index will refer a watched model having the
                ///   particular property indexed in <see cref="PropertyId"/>,
                ///   which must be of type T.
                /// </summary>
                public ushort ContextId;

                /// <summary>
                ///   The id/index of the property being updated, in
                ///   the given context by <see cref="ContextId"/>, for
                ///   an object having such behaviour and being watched.
                /// </summary>
                public ushort PropertyId;

                /// <summary>
                ///   The object's data.
                /// </summary>
                public T Value;

                public void Serialize(Serializer serializer)
                {
                    serializer.Serialize(ref ScopeInstanceIndex);
                    serializer.Serialize(ref ObjectInstanceIndex);
                    serializer.Serialize(ref ContextId);
                    serializer.Serialize(ref PropertyId);
                    Value = Value ?? new T();
                    Value.Serialize(serializer);
                }
            }
        }
    }
}
