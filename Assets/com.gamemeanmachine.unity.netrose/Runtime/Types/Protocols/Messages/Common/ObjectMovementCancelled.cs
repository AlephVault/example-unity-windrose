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
                ///     cancelled its current movement.
                ///   </para>
                /// </summary>
                public class ObjectMovementCancelled : ISerializable
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
                    ///   The to-revert x-position.
                    /// </summary>
                    public ushort RevertToX;

                    /// <summary>
                    ///   The to-revert y-position.
                    /// </summary>
                    public ushort RevertToY;

                    public void Serialize(Serializer serializer)
                    {
                        serializer.Serialize(ref ScopeInstanceIndex);
                        serializer.Serialize(ref ObjectInstanceIndex);
                        serializer.Serialize(ref RevertToX);
                        serializer.Serialize(ref RevertToY);
                    }
                }
            }
        }
    }
}
