using UnityEngine;


namespace AlephVault.Unity.Meetgard.Scopes
{
    namespace Authoring
    {
        namespace Behaviours
        {
            namespace Client
            {
                /// <summary>
                ///   <para>
                ///     The client side implementation of a scope. The purpose
                ///     of this class is to provide a reflection of the content
                ///     that is rendered in the server side of this scope. This
                ///     refers to the objects that will be synchronized from
                ///     the server side.
                ///   </para>
                ///   <para>
                ///     How the in-scope objects will be synchronized, is out
                ///     of scope. But a mean, or a moment in time, will be given.
                ///   </para>
                ///   <para>
                ///     Typically, these scopes will be instantiated out of
                ///     prefabs. Otherwise, some sort of standardized mechanism
                ///     will exist to instantiate this scope and the matching
                ///     server side implementation of this scope.
                ///   </para>
                /// </summary>
                public class ScopeClientSide : MonoBehaviour
                {
                    /// <summary>
                    ///   The id of the current scope. Given by the server.
                    /// </summary>
                    public uint Id { get; private set; }

                    // TODO implement EVERYTHING here.
                }
            }
        }
    }
}
