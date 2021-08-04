using MLAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AlephVault.Unity.MMO
{
    namespace Authoring
    {
        namespace Behaviours
        {
            namespace Scopes
            {
                /// <summary>
                ///   <para>
                ///     These scoped objects have some helper methods
                ///     to self-[un]register from the related network
                ///     scope objects in the hierarchy. These objects
                ///     are initially hidden for all the connections
                ///     but on spawn they trigger a custom criterion
                ///     to make themselves visible to those clients
                ///     watching their containing scope.
                ///   </para>
                /// </summary>
                [RequireComponent(typeof(NetworkObject))]
                public class Scoped : NetworkBehaviour
                {
                    private NetworkScope lastTrackedScope = null;

                    // This function always returns false so the
                    // object is not visible to any connection.
                    // On NetworkStart, however, new definitions
                    // will be used to choose the criterion that
                    // should make it visible to certain clients.
                    private static bool InitiallyHiddenForEveryone(ulong clientId)
                    {
                        return false;
                    }

                    void Awake()
                    {
                        NetworkObject.CheckObjectVisibility = InitiallyHiddenForEveryone;
                    }

                    public override void NetworkStart()
                    {
                        UpdateCurrentScope();
                    }

                    /// <summary>
                    ///   <para>
                    ///     Checks whether the current scope is different
                    ///     to the last kept score, and notifies both
                    ///     scopes (old and new) to update the visibility
                    ///     to their watching clients about this object.
                    ///   </para>
                    ///   <para>
                    ///     This object must be called with special care
                    ///     because it might be expensive on hierarchy
                    ///     traversal.
                    ///   </para>
                    /// </summary>
                    public void UpdateCurrentScope()
                    {
                        NetworkScope newScope = GetComponentInParent<NetworkScope>();
                        if (newScope != lastTrackedScope)
                        {
                            lastTrackedScope?.OnChildRemoved(NetworkObject);
                            newScope?.OnChildAdded(NetworkObject);
                        }
                    }
                }
            }
        }
    }
}
