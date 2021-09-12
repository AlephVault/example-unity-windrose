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
            ///     started moving.
            ///   </para>
            /// </summary>
            public class ObjectMovementStarted : ISerializable
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
                ///   The start x-position.
                /// </summary>
                public ushort StartX;

                /// <summary>
                ///   The start y-position.
                /// </summary>
                public ushort StartY;

                /// <summary>
                ///   The direction of the started movement.
                /// </summary>
                public Direction Direction;

                public void Serialize(Serializer serializer)
                {
                    serializer.Serialize(ref ScopeInstanceIndex);
                    serializer.Serialize(ref ObjectInstanceIndex);
                    serializer.Serialize(ref StartX);
                    serializer.Serialize(ref StartY);
                    serializer.Serialize(ref Direction);
                }
            }
        }
    }
}
