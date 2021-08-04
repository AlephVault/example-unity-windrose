using MLAPI;
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
                ///     Scopes are intended to isolate objects and clients
                ///     in the way that certain objects can only be watched
                ///     by certain clients (e.g. maps). This lightens the
                ///     server load and also prevents cheats from clients.
                ///   </para>
                ///   <para>
                ///     This type of objects is not intended for Host mode,
                ///     since Host mode has an unfair advantage (for it has
                ///     all the objects instantiated in it, which is not the
                ///     case for the remote clients). Nevertheless, host
                ///     modes may fell save to use this method as well.
                ///   </para>
                /// </summary>
                [RequireComponent(typeof(NetworkObject))]
                public class NetworkScope : MonoBehaviour
                {
                    // Keeps a track of the client connections belonging
                    // to a particular scope.
                    private static Dictionary<NetworkScope, HashSet<ulong>> clientsInScopes = new Dictionary<NetworkScope, HashSet<ulong>>();

                    // Keeps a track of the scope a client connection is
                    // tied to.
                    private static Dictionary<ulong, NetworkScope> scopeByClient = new Dictionary<ulong, NetworkScope>();

                    private void Awake()
                    {
                        clientsInScopes[this] = new HashSet<ulong>();
                    }

                    private void OnDestroy()
                    {
                        // Get the current set of observers, and pop it.
                        HashSet<ulong> clientIds = clientsInScopes[this];
                        clientsInScopes.Remove(this);

                        // Remove observer clients from the reverse map.
                        foreach (ulong clientId in clientIds)
                        {
                            scopeByClient.Remove(clientId);
                        }

                        // Ensure any children network object is hidden.
                        // TODO: Test whether this is needed at all.
                        foreach (NetworkObject obj in GetComponentsInChildren<NetworkObject>())
                        {
                            foreach(ulong clientId in clientIds)
                            {
                                obj.NetworkHide(clientId);
                            }
                        }
                    }

                    /// <summary>
                    ///   Adds a client connection to this scope.
                    ///   It adds it to the direct and reverse maps,
                    ///   and also notifies every NetworkObject in
                    ///   the hierarchy to show themselves to that
                    ///   incoming connection.
                    /// </summary>
                    /// <param name="clientId">The id of the client connection to add</param>
                    /// <returns>Whether the client was added to this scope, or it was already added</returns>
                    public bool AddClient(ulong clientId)
                    {
                        if (scopeByClient.ContainsKey(clientId))
                        {
                            return false;
                        }
                        else
                        {
                            // Add the client to the direct map.
                            clientsInScopes[this].Add(clientId);

                            // Add the client to the reverse map.
                            scopeByClient[clientId] = this;

                            // Notify all the children objects.
                            foreach (NetworkObject obj in GetComponentsInChildren<NetworkObject>())
                            {
                                obj.NetworkShow(clientId);
                            }

                            return true;
                        }
                    }

                    /// <summary>
                    ///   Removes a client from this scope, if it
                    ///   was present. Removes from both lists and
                    ///   makes every network object hide from the
                    ///   client being removed.
                    /// </summary>
                    /// <param name="clientId">The id of the client connection to remove</param>
                    /// <returns>Whether the client was removed, or it was never there in first place</returns>
                    public bool RemoveClient(ulong clientId)
                    {
                        if (scopeByClient.ContainsKey(clientId))
                        {
                            // Remove the client to the direct map.
                            clientsInScopes[this].Remove(clientId);

                            // Remove the client to the reverse map.
                            scopeByClient.Remove(clientId);

                            // Notify all the children objects.
                            foreach (NetworkObject obj in GetComponentsInChildren<NetworkObject>())
                            {
                                obj.NetworkHide(clientId);
                            }

                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }

                    /// <summary>
                    ///   This method is assembly-friendly to be
                    ///   invoked by scoped objects to notify when
                    ///   they are added to the hierarchy of a new
                    ///   NetworkScope object. All the clients in
                    ///   the list of watchers for this scope are
                    ///   enabled as watchers of the newly added
                    ///   network object.
                    /// </summary>
                    internal void OnChildAdded(NetworkObject obj)
                    {
                        foreach(ulong clientId in clientsInScopes[this])
                        {
                            obj.NetworkShow(clientId);
                        }
                    }

                    /// <summary>
                    ///   This method is assembly-friendly to be
                    ///   invoked by scoped objects to notify when
                    ///   they are removed from the hierarchy of a
                    ///   NetworkScope object. All the clients in
                    ///   the list of watchers for this scope are
                    ///   disabled as watchers of the just removed
                    ///   network object.
                    /// </summary>
                    /// <param name="obj"></param>
                    internal void OnChildRemoved(NetworkObject obj)
                    {
                        foreach(ulong clientId in clientsInScopes[this])
                        {
                            obj.NetworkHide(clientId);
                        }
                    }
                }
            }
        }
    }
}
