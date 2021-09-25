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
                ///     teleported inside the same map.
                ///   </para>
                /// </summary>
                public class ObjectTeleported : ISerializable
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
}