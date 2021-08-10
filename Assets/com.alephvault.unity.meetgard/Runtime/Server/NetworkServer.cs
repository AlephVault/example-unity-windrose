using AlephVault.Unity.Binary;
using AlephVault.Unity.Meetgard.Types;
using System;
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
        ///   <para>
        ///     Additionally, a local connection ("host") is allowed in
        ///     a per-server basis.
        ///   </para>
        /// </summary>
        public class NetworkServer : MonoBehaviour
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

            // Adds a host endpoint (which is newly instantiated).
            // TODO: replace NetworkEndpoint with NetworkLocalEndpoint.
            private void AddHostEndpoint(NetworkEndpoint endpoint)
            {
                endpointById.Add(HostEndpointId, endpoint);
                endpointIds.Add(endpoint, HostEndpointId);
            }

            // Removes the host endpoint. It will emulate disconnection
            // events as if it were a remote endpoint.
            private void RemoveHostEndpoint()
            {
                if (endpointById.TryGetValue(HostEndpointId, out NetworkEndpoint endpoint))
                {
                    endpointById.Remove(HostEndpointId);
                    endpointIds.Remove(endpoint);
                }
            }

            /// <summary>
            ///   <para>
            ///     This event is triggered after a server successfully
            ///     started (right after successfully start listening
            ///     and accepting incoming connections).
            ///   </para>
            ///   <para>
            ///     This event is triggered in an asynchronous context.
            ///     Any game load should be done before starting it so
            ///     race conditions between new connections and game
            ///     load state do not occur. However, if there are
            ///     no issues regarding that, the game load could
            ///     occur also here.
            ///   </para>
            /// </summary>
            public event Action OnServerStarted = null;

            /// <summary>
            ///   <para>
            ///     This event is triggered after an incoming connection
            ///     was accepted, and registered (and an ID was given).
            ///     This detects both remote and local (host) connections
            ///     (in such cases, it will pass <see cref="HostEndpointId"/>
            ///     as argument).
            ///   </para>
            ///   <para>
            ///     This event is triggered in an asynchronous context.
            ///   </para>
            /// </summary>
            public event Action<ulong> OnClientConnected = null;

            /// <summary>
            ///   <para>
            ///     This event is triggered after a client message arrives.
            ///     The arguments for this message are: client id, protocol id,
            ///     message tag, and a buffer reader with the contents.
            ///   </para>
            ///   <para>
            ///     PLEASE NOTE: ONLY ONE HANDLER SHOULD HANDLE THE INCOMING MESSAGE, AND IT
            ///     SHOULD EXHAUST THE BUFFER COMPLETELY.
            ///   </para>
            ///   <para>
            ///     This event is triggered in an asynchronous context.
            ///   </para>
            /// </summary>
            public event Action<ulong, ushort, ushort, Reader> OnMessage = null;

            /// <summary>
            ///   <para>
            ///     This event is triggered after a client is disconnected.
            ///     Such client can be a remote endpoint or the local (host)
            ///     endpoint (in such cases, it will pass <see cref="HostEndpointId"/>
            ///     as argument).
            ///   </para>
            ///   <para>
            ///     This event is triggered in an asynchronous context.
            ///   </para>
            /// </summary>
            public event Action<ulong> OnClientDisconnected = null;

            /// <summary>
            ///   <para>
            ///     This event is triggered when a server was told to stop.
            ///     This typically occurs as an error while accepting new
            ///     connections (other errors are in a per-connection basis)
            ///     or when the server Was told to close (in this case, the
            ///     exception will be null). All of the existing endpoints
            ///     were told to close (this does not mean that the respective
            ///     disconnection events were processed for them) before
            ///     this event is triggered. There is nothing to veto here,
            ///     specially in per-connection basis, but just doing a
            ///     global cleanup of the whole server.
            ///   </para>
            ///   <para>
            ///     This event is triggered in an asynchronous context.
            ///   </para>
            /// </summary>
            public event Action<System.Exception> OnServerStopping = null;
        }
    }
}
