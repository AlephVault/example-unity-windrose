namespace GameMeanMachine.Unity.NetRose
{
    namespace Types
    {
        namespace Protocols
        {
            using AlephVault.Unity.Binary;

            /// <summary>
            ///   <para>
            ///     This message tells the client that right now
            ///     it will stop receiving updates from the object
            ///     in a particular context. Typically, this message
            ///     is received when stopping receiving inventory
            ///     updates (from chest-like objects) or other types
            ///     of close-look of an object (it will seldom to
            ///     never happen to owned objects).
            ///   </para>
            ///   <para>
            ///     This message will be received in three possible
            ///     moments: Two of them involving the object being
            ///     despawned (this message will be sent BEFORE the
            ///     "despawn" message, and once for each context),
            ///     and the third one involving the particular need
            ///     to stop watching one particular object in one
            ///     particular context.
            ///   </para>
            /// </summary>
            public class ObjectUnwatched : ISerializable
            {
                /// <summary>
                ///   The server-side index of the scope the
                ///   client is connected to, and the object
                ///   belongs to.
                /// </summary>
                public uint ScopeInstanceIndex;

                /// <summary>
                ///   The server-side index of the object being
                ///   unwatched by the client.
                /// </summary>
                public uint ObjectInstanceIndex;

                /// <summary>
                ///   The id/index of the context this object is
                ///   being unwatched. Objects have a default context
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
                ///   the 0-based index of the behaviour being unwatched.
                /// </summary>
                public ushort ContextId;

                public void Serialize(Serializer serializer)
                {
                    serializer.Serialize(ref ScopeInstanceIndex);
                    serializer.Serialize(ref ObjectInstanceIndex);
                    serializer.Serialize(ref ContextId);
                }
            }
        }
    }
}
