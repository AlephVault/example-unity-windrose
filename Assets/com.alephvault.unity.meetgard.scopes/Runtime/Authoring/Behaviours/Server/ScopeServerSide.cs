using UnityEngine;


namespace AlephVault.Unity.Meetgard.Scopes
{
    namespace Authoring
    {
        namespace Behaviours
        {
            namespace Server
            {
                /// <summary>
                ///   <para>
                ///     The server side implementation of a scope. The purpose
                ///     of this class is to provide both a space to group many
                ///     connections to communicate between themselves (by being
                ///     them on the same hierarchy).
                ///   </para>
                ///   <para>
                ///     How the in-scope objects will be synchronized, is out
                ///     of scope. But a mean, or a moment in time, will be given.
                ///   </para>
                ///   <para>
                ///     Typically, these scopes will be instantiated out of
                ///     prefabs. Otherwise, some sort of standardized mechanism
                ///     will exist to instantiate this scope and the matching
                ///     client side implementation of this scope.
                ///   </para>
                /// </summary>
                public class ScopeServerSide : MonoBehaviour
                {
                }
            }
        }
    }
}
