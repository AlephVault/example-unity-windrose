namespace GameMeanMachine.Unity.NetRose
{
    namespace Types
    {
        namespace Protocols
        {
            using AlephVault.Unity.Binary;
            using GameMeanMachine.Unity.WindRose.Types;

            /// <summary>
            ///   <para>
            ///     This message tells the client that an object
            ///     is being changed the orientation.
            ///   </para>
            /// </summary>
            public class ObjectOrientationChanged : ISerializable
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
                ///   The new orientation.
                /// </summary>
                public Direction Orientation;

                public void Serialize(Serializer serializer)
                {
                    serializer.Serialize(ref ScopeInstanceIndex);
                    serializer.Serialize(ref ObjectInstanceIndex);
                    serializer.Serialize(ref Orientation);
                }
            }
        }
    }
}
