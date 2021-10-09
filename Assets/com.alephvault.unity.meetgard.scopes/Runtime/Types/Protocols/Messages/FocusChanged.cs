using AlephVault.Unity.Binary;


namespace AlephVault.Unity.Meetgard.Scopes
{
    namespace Types
    {
        namespace Protocols
        {
            namespace Messages
            {
                /// <summary>
                ///   A message telling the connection changed its focus
                ///   to an object in the new scope. This focus message
                ///   may/will be received even before the target object
                ///   is spawned locally. The client will be aware of it
                ///   by using some sort of "wait" logic for that object,
                ///   or until another FocusChanged message is received
                ///   (or even a <see cref="FocusReleased"/> message, if
                ///   the case).
                /// </summary>
                public class FocusChanged : ISerializable
                {
                    /// <summary>
                    ///   The effective index of the current scope, sent
                    ///   for validity. This message should be discarded
                    ///   if the scope is no more valid.
                    /// </summary>
                    public uint ScopeIndex;

                    /// <summary>
                    ///   The effective index of the object to focus on
                    ///   in the current scope.
                    /// </summary>
                    public uint ObjectIndex;

                    public void Serialize(Serializer serializer)
                    {
                        serializer.Serialize(ref ScopeIndex);
                        serializer.Serialize(ref ObjectIndex);
                    }
                }
            }
        }
    }
}