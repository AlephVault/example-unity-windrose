namespace GameMeanMachine.Unity.NetRose
{
    namespace Types
    {
        namespace Protocols
        {
            using AlephVault.Unity.Binary;

            /// <summary>
            ///   This message tells the client that an object
            ///   was attached to a map, inside a scope the
            ///   client is watching.
            /// </summary>
            public class ObjectAttached : ISerializable
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
                ///   The index of the map, inside the scope, the
                ///   object was attached to.
                /// </summary>
                public byte MapIndex;

                /// <summary>
                ///   The target x-position.
                /// </summary>
                public ushort TargetX;

                /// <summary>
                ///   The target y-position.
                /// </summary>
                public ushort TargetY;

                public void Serialize(Serializer serializer)
                {
                    serializer.Serialize(ref ScopeInstanceIndex);
                    serializer.Serialize(ref ObjectInstanceIndex);
                    serializer.Serialize(ref TargetX);
                    serializer.Serialize(ref TargetY);
                }
            }
        }
    }
}
