using AlephVault.Unity.Binary;
using AlephVault.Unity.Meetgard.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace AlephVault.Unity.Meetgard
{
    namespace Server
    {
        public partial class NetworkServer : MonoBehaviour
        {
            // The endpoint id for the host.
            public const ulong HostEndpointId = 0;

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

            // The next id to use, when a new connection is spawned.
            // Please note: id=0 is reserved for a single network
            // endpoint of type NetworkHostEndpoint (i.e. the host
            // connection for non-dedicated games).
            private ulong nextEndpointId = 1;

            // A mapping of the connections currently established. Each
            // connection is mapped against a generated id for them.
            private Dictionary<NetworkEndpoint, ulong> endpointIds = new Dictionary<NetworkEndpoint, ulong>();

            // A mapping of the connections by their ids.
            private SortedDictionary<ulong, NetworkEndpoint> endpointById = new SortedDictionary<ulong, NetworkEndpoint>();

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
                            throw new Types.Exception("Connections exhausted! The server is insanely and improbably full");
                        }
                        if (!endpointById.ContainsKey(testId)) return testId;
                        testId++;
                    }
                }
            }

            // Removes the host endpoint. It will emulate disconnection
            // events as if it were a remote endpoint.
            private async void RemoveHostEndpoint()
            {
                if (endpointById.TryGetValue(HostEndpointId, out NetworkEndpoint endpoint))
                {
                    endpointById.Remove(HostEndpointId);
                    endpointIds.Remove(endpoint);
                    TriggerOnClientDisconnected(HostEndpointId, null);
                }
            }

            // Creates a NetworkRemoteEndpoint for the given client
            // socket (which is a just-accepted socket), and adds
            // it to the registered endpoints. This is ran on the
            // main server life-cycle.
            private void AddNetworkClientEndpoint(TcpClient clientSocket)
            {
                ulong nextId = GetNextEndpointId();
                NetworkEndpoint endpoint = new NetworkRemoteEndpoint(clientSocket, () =>
                {
                    TriggerOnClientConnected(nextId);
                }, (protocolId, messageTag, reader) =>
                {
                    TriggerOnMessage(nextId, protocolId, messageTag, reader);
                }, (e) =>
                {
                    NetworkEndpoint endpoint = endpointById[nextId];
                    endpointById.Remove(nextId);
                    endpointIds.Remove(endpoint);
                    TriggerOnClientDisconnected(nextId, e);
                }, maxMessageSize, trainBoardingTime, idleSleepTime);
                endpointById.Add(nextId, endpoint);
                endpointIds.Add(endpoint, nextId);
            }

            // Creates a NetworkLocalEndpoint, on request, and adds it
            // to the registered endpoints. This is ran on demand.
            private void AddNetworkHostEndpoint()
            {
                /**
                 * TODO implement this... later.
                 * 
                NetworkLocalEndpoint endpoint = new NetworkLocalEndpoint(() =>
                {
                    TriggerOnClientConnected(HostEndpointId);
                }, (protocolId, messageTag, reader) =>
                {
                    TriggerOnMessage(HostEndpointId, protocolId, messageTag, reader);
                }, () =>
                {
                    TriggerOnClientDisconnected(HostEndpointId, null);
                });
                 */
            }

            /// <summary>
            ///   Starts a host endpoint (only allowed on an already running server).
            /// </summary>
            public void StartHostEndpoint()
            {
                if (!IsListening)
                {
                    throw new InvalidOperationException("The server is not listening - host endpoint cannot be created");
                }

                AddNetworkHostEndpoint();
            }

            /// <summary>
            ///   Checks whether an endpoint with the given is registered.
            /// </summary>
            /// <param name="clientId">The id to check</param>
            /// <returns>Whether the endpoint exists</returns>
            public bool EndpointExists(ulong clientId)
            {
                return endpointById.ContainsKey(clientId);
            }
        }
    }
}
