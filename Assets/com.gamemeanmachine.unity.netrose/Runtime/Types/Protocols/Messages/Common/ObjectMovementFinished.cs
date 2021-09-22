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
                ///     completed moving.
                ///   </para>
                /// </summary>
                public class ObjectMovementFinished : ISerializable
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
                    ///   The end x-position.
                    /// </summary>
                    public ushort EndX;

                    /// <summary>
                    ///   The end y-position.
                    /// </summary>
                    public ushort EndY;

                    public void Serialize(Serializer serializer)
                    {
                        serializer.Serialize(ref ScopeInstanceIndex);
                        serializer.Serialize(ref ObjectInstanceIndex);
                        serializer.Serialize(ref EndX);
                        serializer.Serialize(ref EndY);
                    }
                }
            }
        }
    }
}
