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
                ///   A message telling the connection released its focus
                ///   in the current scope. The client must drop the local
                ///   implementation of the focus, considering that the
                ///   object might be still alive, however (it must do the
                ///   same when the object it was focused on is being
                ///   despawned, but in this case, this is an explicit
                ///   mandate of unfocusing regardless any cause).
                /// </summary>
                public class FocusReleased : ISerializable
                {
                    /// <summary>
                    ///   The effective index of the current scope, sent
                    ///   for validity. This message should be discarded
                    ///   if the scope is no more valid.
                    /// </summary>
                    public uint ScopeIndex;

                    public void Serialize(Serializer serializer)
                    {
                        serializer.Serialize(ref ScopeIndex);
                    }
                }
            }
        }
    }
}