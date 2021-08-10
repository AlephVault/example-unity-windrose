using AlephVault.Unity.Meetgard.Types;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlephVault.Unity.Meetgard
{
    namespace Server
    {
        /// <summary>
        ///   <para>
        ///     Network servers are behaviours that spawn an additional
        ///     thread to listen for connections. Each connection is
        ///     accepted and, for each one, a new thread is spawned to
        ///     handle it. Each server can listen in one address:port
        ///     at once, but many different servers can be instantiated
        ///     in the same scene.
        ///   </para>
        /// </summary>
        public class NetworkServer : MonoBehaviour
        {
            /// <summary>
            ///   <para>
            ///     The time to sleep, on each iteration, when no data to
            ///     read or write is present in the socket on a given
            ///     iteration.
            ///   </para>
            ///   <para>
            ///     This setting should match whatever is set in the clients
            ///     and supported by the protocols to use.
            ///   </para>
            /// </summary>
            [SerializeField]
            private float idleSleepTime = 0.01f;

            /// <summary>
            ///   <para>
            ///     The maximum size of each individual message to be sent.
            ///   </para>
            ///   <para>
            ///     This setting should match whatever is set in the clients
            ///     and supported by the protocols to use.
            ///   </para>
            /// </summary>
            [SerializeField]
            private ushort maxMessageSize = 1024;

            /// <summary>
            ///   <para>
            ///     The time an endpoint waits for more data after some
            ///     message data was sent to the internal outgoing messages
            ///     buffer.
            ///   </para>
            ///   <para>
            ///     This setting should match whatever is set in the clients
            ///     and supported by the protocols to use.
            ///   </para>
            /// </summary>
            [SerializeField]
            private float trainBoardingTime = 0.75f;

            // The next id to use, when a new connection is spawned.
            // Please note: id=0 is reserved for a single network
            // endpoint of type NetworkHostEndpoint (i.e. the host
            // connection for non-dedicated games).
            private ulong nextEndpointId = 1;

            // A mapping of the connections currently established. Each
            // connection is mapped against a generated id for them.
            private Dictionary<NetworkRemoteEndpoint, ulong> endpointIds = new Dictionary<NetworkRemoteEndpoint, ulong>();

            // A mapping of the connections by their ids.
            private SortedDictionary<ulong, NetworkRemoteEndpoint> endpointById = new SortedDictionary<ulong, NetworkRemoteEndpoint>();

            // Gets the next id to use. If the next endpoint id is the
            // maximum value, it tries searching a free id among the
            // mapping keys. Otherwise, it just returns the value and
            // then increments.
            private ulong GetNextEndpointId()
            {
                if (nextEndpointId < ulong.MaxValue)
                {
                    return nextEndpointId++;
                }
                else
                {
                    ulong testId = 1;
                    while(true)
                    {
                        if (testId == ulong.MaxValue)
                        {
                            throw new Exception("Connections exhausted! The server is insanely and improbably full");
                        }
                        if (!endpointById.ContainsKey(testId)) return testId;
                        testId++;
                    }
                }
            }

            // Adds an endpoint (which is newly instantiated/connected)
            // to the list of endpoints connected to this server.
            private void AddClientEndpoint(NetworkRemoteEndpoint endpoint)
            {
                ulong nextId = GetNextEndpointId();
                endpointById.Add(nextId, endpoint);
                endpointIds.Add(endpoint, nextId);
            }

            // Removes an endpoint (which is just disconnected) from
            // the list of endpoints connected to this server.
            private void RemoveClientEndpoint(NetworkRemoteEndpoint endpoint)
            {
                if (endpointIds.TryGetValue(endpoint, out ulong id))
                {
                    endpointById.Remove(id);
                    endpointIds.Remove(endpoint);
                }
            }
        }
    }
}
