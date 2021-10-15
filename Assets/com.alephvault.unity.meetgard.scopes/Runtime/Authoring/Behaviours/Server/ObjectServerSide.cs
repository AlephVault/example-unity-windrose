using AlephVault.Unity.Meetgard.Scopes.Types.Constants;
using AlephVault.Unity.Support.Authoring.Behaviours;
using System;
using System.Threading.Tasks;
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
                ///   The server side implementation of an object that must
                ///   exist both in the client and the server. This involves
                ///   also knowing which prefab id this object is registered
                ///   with, in a specific server making use of it, and even
                ///   a specific scope inside the server, if the object is
                ///   created into that server but spawned into no particular
                ///   server side scope.
                /// </summary>
                public class ObjectServerSide : MonoBehaviour
                {
                    // These two fields are set by the protocol.

                    /// <summary>
                    ///   The id/index of the internal server-side registered
                    ///   prefab object this object is associated to. Typically,
                    ///   this means that an object is created in a particular
                    ///   server and with a particular prefab therein. This
                    ///   means that this value is meaningless, even when being
                    ///   zero, if <see cref="Protocol"/> is not set.
                    /// </summary>
                    public uint PrefabId { get; internal set; }

                    /// <summary>
                    ///   An optional key to be used. This is only meaningful
                    ///   if the object this key is assigned to, is actually a
                    ///   prefab object inside a specific server. When this
                    ///   value is set (in constrast to null or ""), it will be
                    ///   added to an internal dictionary of prefabs by their
                    ///   keys, and thus be available to be instantiated via
                    ///   a method taking its key instead of its index.
                    /// </summary>
                    [SerializeField]
                    private string prefabKey;

                    /// <summary>
                    ///   See <see cref="prefabKey"/>.
                    /// </summary>
                    public string PrefabKey => prefabKey;

                    /// <summary>
                    ///   The protocol this object is associated to. Typically,
                    ///   this means that an object is created in that particular
                    ///   server and with a particular prefab therein. This
                    ///   means that this value is associated with the value in
                    ///   the <see cref="PrefabId"/> field.
                    /// </summary>
                    public ScopesProtocolServerSide Protocol { get; internal set; }

                    // This field is set by the owning scope.

                    /// <summary>
                    ///   The id this object is assigned to inside a particular
                    ///   scope. This also requires a particular scope to be set
                    ///   in the <see cref="Scope"/> field. Also, the scope being
                    ///   assigned will belong to the same protocol that was given
                    ///   as value in the <see cref="Protocol"/> field. This
                    ///   means that this value is meaningless, even when being
                    ///   zero, if <see cref="Protocol"/> is not set.
                    /// </summary>
                    public uint Id { get; internal set; }

                    /// <summary>
                    ///   If the object is spawned, this field will have which
                    ///   scope this object is spawned into. If the object belongs
                    ///   to a server but it is not spawned, this value will be
                    ///   null. When this value is set, the <see cref="Id"/> field
                    ///   will have a meaninful value: the id this object was given
                    ///   when spawning this object (and also populating this field).
                    /// </summary>
                    public ScopeServerSide Scope { get; internal set; }
                }
            }
        }
    }
}

