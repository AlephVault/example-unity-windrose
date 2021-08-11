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
            }

            // The current server life-cycle.
            private Thread lifeCycle = null;

            // The current listener.
            private TcpListener listener = null;

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
            ///     Please note: for id <see cref="HostEndpointId"/>, the
            ///     involved client is the local / host one.
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
            ///     Please note: for id <see cref="HostEndpointId"/>, the
            ///     involved client is the local / host one.
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
            ///     as argument). A non-null exception means that the closure
            ///     was not graceful, but due to an internal error in the
            ///     endpoint connection lifecycle (only meaningful for
            ///     remote endpoints, not the local/host one).
            ///   </para>
            ///   <para>
            ///     Please note: for id <see cref="HostEndpointId"/>, the
            ///     involved client is the local / host one.
            ///   </para>
            ///   <para>
            ///     This event is triggered in an asynchronous context.
            ///   </para>
            /// </summary>
            public event Action<ulong, System.Exception> OnClientDisconnected = null;

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
            public event Action<System.Exception> OnServerStopped = null;

            /// <summary>
            ///   Tells whether the life-cycle is active or not. While Active, another
            ///   life-cycle (e.g. a call to <see cref="Listen(int)"/> or
            ///   <see cref="Connect(string, int)"/>) cannot be done.
            /// </summary>
            public bool IsRunning { get { return lifeCycle != null && lifeCycle.IsAlive; } }

            /// <summary>
            ///   Tells whether the server is currently listening.
            /// </summary>
            public bool IsListening { get { return listener != null; } }

            /// <summary>
            ///   Starts the server, if it is not already started, in all the
            ///   available ip network interfaces.
            /// </summary>
            /// <param name="port">The port to listen at</param>
            public void StartServer(int port)
            {
                StartServer(IPAddress.Any, port);
            }

            /// <summary>
            ///   Starts the server, if it is not already started.
            /// </summary>
            /// <param name="adddress">The address to listen at</param>
            /// <param name="port">The port to listen at</param>
            public void StartServer(IPAddress address, int port)
            {
                if (IsRunning)
                {
                    throw new InvalidOperationException("The server is already running");
                }

                listener = new TcpListener(address, port);
                listener.Start();
                lifeCycle = new Thread(new ThreadStart(LifeCycle));
                lifeCycle.IsBackground = true;
                lifeCycle.Start();
            }

            /// <summary>
            ///   Stops the server, if it is already started and listening.
            ///   This will trigger an exception in the life-cycle which will
            ///   be understood as a graceful closure.
            /// </summary>
            public void StopServer()
            {
                if (!IsListening)
                {
                    throw new InvalidOperationException("The server is not listening");
                }

                listener.Stop();
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

            /// <summary>
            ///   Sends a message to a registered endpoint by its id.
            /// </summary>
            /// <param name="clientId">The id of the client</param>
            /// <param name="protocolId">The id of protocol for this message</param>
            /// <param name="messageTag">The tag of the message being sent</param>
            /// <param name="input">The input stream</param>
            public async Task Send(ulong clientId, ushort protocolId, ushort messageTag, Stream input)
            {
                if (!IsRunning)
                {
                    throw new InvalidOperationException("The server is not running - cannot send any message");
                }

                await endpointById[clientId].Send(protocolId, messageTag, input);
            }

            /// <summary>
            ///   Sends a message to a registered endpoint by its id.
            ///   If the endpoint is not found, <code>false</code> is
            ///   returned instead of raising an exception.
            /// </summary>
            /// <param name="clientId">The id of the client</param>
            /// <param name="protocolId">The id of protocol for this message</param>
            /// <param name="messageTag">The tag of the message being sent</param>
            /// <param name="input">The input stream</param>
            public async Task<bool> TrySend(ulong clientId, ushort protocolId, ushort messageTag, Stream input)
            {
                if (!IsRunning)
                {
                    throw new InvalidOperationException("The server is not running - cannot send any message");
                }

                if (endpointById.TryGetValue(clientId, out NetworkEndpoint value))
                {
                    await value.Send(protocolId, messageTag, input);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            /// <summary>
            ///   <para>
            ///     Sends a message to many registered endpoints by their ids.
            ///     All the endpoints that are not found, or throw an exception
            ///     on send, are ignored and kept in an output bag of failed
            ///     endpoints.
            ///   </para>
            ///   <para>
            ///     Notes: use <code>null</code> as the first argument to notify
            ///     to all the available registered endpoints.
            ///   </para>
            /// </summary>
            /// <param name="clientIds">The ids to send the same message - use null to specify ALL the available ids</param>
            /// <param name="protocolId">The id of protocol for this message</param>
            /// <param name="messageTag">The tag of the message being sent</param>
            /// <param name="input">The input stream</param>
            /// <param name="failedEndpoints">The output list of the endpoints that are not found or raised an error on send</param>
            public async Task TryBroadcast(ulong[] clientIds, ushort protocolId, ushort messageTag, Stream input, HashSet<ulong> failedEndpoints)
            {
                if (!IsRunning)
                {
                    throw new InvalidOperationException("The server is not running - cannot send any message");
                }

                Binary.Buffer buffer;
                if (input is Binary.Buffer buffer2)
                {
                    buffer = buffer2;
                }
                else
                {
                    // The buffer is meant to be copied.
                    buffer = new Binary.Buffer(new Reader(input).ReadByteArray(new byte[input.Length]));
                }

                if (failedEndpoints != null) failedEndpoints.Clear();

                if (clientIds == null)
                {
                    foreach (ulong clientId in clientIds)
                    {
                        if (endpointById.TryGetValue(clientId, out NetworkEndpoint endpoint))
                        {
                            try
                            {
                                await endpoint.Send(protocolId, messageTag, buffer);
                            }
                            catch
                            {
                                if (failedEndpoints != null) failedEndpoints.Add(clientId);
                            }
                            buffer.Seek(0, SeekOrigin.Begin);
                        }
                        else
                        {
                            if (failedEndpoints != null) failedEndpoints.Add(clientId);
                        }
                    }
                }
                else
                {
                    foreach(KeyValuePair<ulong, NetworkEndpoint> pair in endpointById.ToArray())
                    {
                        try
                        {
                            await pair.Value.Send(protocolId, messageTag, buffer);
                        }
                        catch
                        {
                            if (failedEndpoints != null) failedEndpoints.Add(pair.Key);
                        }
                        buffer.Seek(0, SeekOrigin.Begin);
                    }
                }
            }

            /// <summary>
            ///   Closes a registered endpoint by its id.
            /// </summary>
            /// <param name="clientId">The id of the client</param>
            public void Close(ulong clientId)
            {
                if (!IsRunning)
                {
                    throw new InvalidOperationException("The server is not running - cannot close any connection");
                }

                endpointById[clientId].Close();
            }

            /// <summary>
            ///   Closes a registered endpoint by its id.
            ///   If the endpoint is not found, <code>false</code> is
            ///   returned instead of raising an exception.
            /// </summary>
            /// <param name="clientId"></param>
            /// <returns>Whether an endpoint existed with that id, and was closed</returns>
            public bool TryClose(ulong clientId)
            {
                if (!IsRunning)
                {
                    throw new InvalidOperationException("The server is not running - cannot close any connection");
                }

                if (endpointById.TryGetValue(clientId, out NetworkEndpoint value))
                {
                    value.Close();
                    return true;
                }
                else
                {
                    return false;
                }
            }

            // Asynchronously triggers the OnServerStarted event.
            private async void TriggerOnServerStarted()
            {
                OnServerStarted?.Invoke();
            }

            // Triggers the OnClientConnected event. This occurs in an asynchronous
            // context, already.
            private void TriggerOnClientConnected(ulong clientId)
            {
                OnClientConnected?.Invoke(clientId);
            }

            // Triggers the OnMessage event. This occurs in an asynchronous context,
            // already.
            private void TriggerOnMessage(ulong clientId, ushort protocolId, ushort messageTag, Reader content)
            {
                OnMessage?.Invoke(clientId, protocolId, messageTag, content);
            }

            // Triggers the OnClientDisconnected event. This occurs in an asynchronous
            // context, already, and after the client disconnected and was removed from
            // the internal endpoints registry.
            private void TriggerOnClientDisconnected(ulong clientId, System.Exception reason)
            {
                OnClientDisconnected?.Invoke(clientId, reason);
            }

            // Asynchronously triggers the OnServerStopped event, but after telling
            // all of the active sockets to close. The server stopped event may encounter
            // race conditions with the disconnection events (which become, in turn, calls
            // to TriggerOnClientDisconnected).
            private async void TriggerOnServerStopped(System.Exception e)
            {
                ulong[] keys = endpointById.Keys.ToArray();
                foreach(ulong key in keys)
                {
                    if (endpointById.TryGetValue(key, out NetworkEndpoint value)) value.Close();
                }
                OnServerStopped?.Invoke(e);
            }

            // The full server life-cycle goes here.
            private void LifeCycle()
            {
                System.Exception lifeCycleException = null;
                try
                {
                    // The server is considered connected right now.
                    TriggerOnServerStarted();
                    // Accepts all of the incoming connections, ad eternum.
                    while(true) AddNetworkClientEndpoint(listener.AcceptTcpClient());
                }
                catch(SocketException e)
                {
                    // If the error code is SocketError.Interrupted, this close reason is
                    // graceful in this context. Otherwise, it is abnormal.
                    if (e.SocketErrorCode != SocketError.Interrupted) lifeCycleException = e;
                }
                catch(System.Exception e)
                {
                    lifeCycleException = e;
                }
                finally
                {
                    if (listener != null)
                    {
                        listener.Stop();
                        listener = null;
                    }
                    TriggerOnServerStopped(lifeCycleException);
                }
            }
        }
    }
}
