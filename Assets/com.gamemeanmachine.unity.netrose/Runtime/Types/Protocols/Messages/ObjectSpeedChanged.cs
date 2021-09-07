namespace GameMeanMachine.Unity.NetRose
{
    namespace Types
    {
        namespace Protocols
        {
            using AlephVault.Unity.Binary;

            /// <summary>
            ///   This message tells the client that an object
            ///   is being changed the speed.
            /// </summary>
            public class ObjectSpeedChanged : ISerializable
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
                ///   The new speed.
                /// </summary>
                public uint Speed;

                public void Serialize(Serializer serializer)
                {
                    serializer.Serialize(ref ScopeInstanceIndex);
                    serializer.Serialize(ref ObjectInstanceIndex);
                    serializer.Serialize(ref Speed);
                }
            }
        }
    }
}
