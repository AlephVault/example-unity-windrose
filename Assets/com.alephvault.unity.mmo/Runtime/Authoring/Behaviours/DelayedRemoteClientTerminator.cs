using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using AlephVault.Unity.Support.Utils;

namespace AlephVault.Unity.MMO
{
    namespace Authoring
    {
        namespace Behaviours
        {
            using System.Threading.Tasks;
            using Types;

            /// <summary>
            ///   <para>
            ///     Adds the ability to delay the termination of
            ///     a client connection. This is useful when we
            ///     want to delay the connection a bit and still
            ///     be able to send a "goodbye" message (e.g.
            ///     when we're kicking a user or rejecting a
            ///     login process).
            ///   </para>
            ///   <para>
            ///     Notes: This class is only useful in the
            ///     context of this bug: https://github.com/Unity-Technologies/com.unity.multiplayer.mlapi/issues/796.
            ///   </para>
            /// </summary>
            [RequireComponent(typeof(NetworkManager))]
            public class DelayedRemoteClientTerminator : MonoBehaviour
            {
                private NetworkManager manager;

                /// <summary>
                ///   The default delay to use when invoking
                /// </summary>
                [SerializeField]
                private float defaultDisconnectionDelay = 0.1f;

                // A set with all of the connections awaiting to be removed.
                private HashSet<ulong> disconnectionPendingClients = new HashSet<ulong>();

                void Start()
                {
                    manager = GetComponent<NetworkManager>();
                    manager.OnClientDisconnectCallback += Manager_OnClientDisconnectCallback;
                    defaultDisconnectionDelay = Values.Max(0.001f, defaultDisconnectionDelay);
                }

                private void Manager_OnClientDisconnectCallback(ulong clientId)
                {
                    // Here we cleanup the client id from the "removal pending"
                    // clients set.
                    disconnectionPendingClients.Remove(clientId);
                }

                /// <summary>
                ///   Tells whether a remote client was required to be disconnected.
                /// </summary>
                /// <param name="clientId">The id of the client connection to query</param>
                /// <returns>Whether it is pending disconnection</returns>
                public bool IsBeingDisconnected(ulong clientId)
                {
                    return disconnectionPendingClients.Contains(clientId);
                }

                /// <summary>
                ///   Issues a remote client disconnection using the default delay.
                /// </summary>
                /// <param name="clientId">The id of the connection to disconnect</param>
                public void DelayedDisconnectClient(ulong clientId)
                {
                    DelayedDisconnectClient(clientId, defaultDisconnectionDelay);
                }

                /// <summary>
                ///   Issues a remote client disconnection using a custom delay.
                ///   It is wrong to use a non-positive delay. In that case, the
                ///   default delay will be used instead.
                /// </summary>
                /// <param name="clientId">The id of the connection to disconnect</param>
                /// <param name="delay">The delay to use</param>
                public void DelayedDisconnectClient(ulong clientId, float delay)
                {
                    if (delay <= 0) delay = defaultDisconnectionDelay;
                    if (!manager.IsServer)
                    {
                        throw new Exception("Disconnecting a client can only be done in server");
                    }
                    else if (clientId == manager.ServerClientId)
                    {
                        throw new Exception("Cannot specify the server id as a client to disconnect");
                    }
                    else if (clientId == manager.LocalClientId)
                    {
                        throw new Exception("Cannot disconnect the local client");
                    }
                    DoDelayedClientDisconnect(clientId, delay);
                }

                // This is the whole callback of a delayed disconnection.
                private async void DoDelayedClientDisconnect(ulong clientId, float delay)
                {
                    // Add the client to the "pending" set, then wait, and then remove.
                    disconnectionPendingClients.Add(clientId);
                    await Task.Delay((int)(1000f * delay));
                    manager.DisconnectClient(clientId);
                }
            }
        }
    }
}
