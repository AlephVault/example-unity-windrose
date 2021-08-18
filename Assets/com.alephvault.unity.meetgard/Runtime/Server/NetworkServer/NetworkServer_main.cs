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
        public partial class NetworkServer : MonoBehaviour
        {
            // The current listener.
            private TcpListener listener = null;

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
                StartLifeCycle();
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
            ///   Sends a message to a registered endpoint by its id.
            /// </summary>
            /// <param name="clientId">The id of the client</param>
            /// <param name="protocolId">The id of protocol for this message</param>
            /// <param name="messageTag">The tag of the message being sent</param>
            /// <param name="input">The input stream</param>
            public async Task Send(ulong clientId, ushort protocolId, ushort messageTag, byte[] content, int length)
            {
                if (!IsRunning)
                {
                    throw new InvalidOperationException("The server is not running - cannot send any message");
                }

                await endpointById[clientId].Send(protocolId, messageTag, content, length);
            }

            /// <summary>
            ///   Sends a message to a registered endpoint by its id.
            ///   If the endpoint is not found, <code>false</code> is
            ///   returned instead of raising an exception.
            /// </summary>
            /// <param name="clientId">The id of the client</param>
            /// <param name="protocolId">The id of protocol for this message</param>
            /// <param name="messageTag">The tag of the message being sent</param>
            /// <param name="content">The input array, typically with a non-zero capacity</param>
            /// <param name="length">The actual length of the content in the array</param>
            public async Task<bool> TrySend(ulong clientId, ushort protocolId, ushort messageTag, byte[] content, int length)
            {
                if (!IsRunning)
                {
                    throw new InvalidOperationException("The server is not running - cannot send any message");
                }

                if (endpointById.TryGetValue(clientId, out NetworkEndpoint value))
                {
                    await value.Send(protocolId, messageTag, content, length);
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
            /// <param name="content">The input array, typically with a non-zero capacity</param>
            /// <param name="length">The actual length of the content in the array</param>
            /// <param name="failedEndpoints">The output list of the endpoints that are not found or raised an error on send</param>
            public async Task TryBroadcast(ulong[] clientIds, ushort protocolId, ushort messageTag, byte[] content, int length, HashSet<ulong> failedEndpoints)
            {
                if (!IsRunning)
                {
                    throw new InvalidOperationException("The server is not running - cannot send any message");
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
                                await endpoint.Send(protocolId, messageTag, content, length);
                            }
                            catch
                            {
                                if (failedEndpoints != null) failedEndpoints.Add(clientId);
                            }
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
                            await pair.Value.Send(protocolId, messageTag, content, length);
                        }
                        catch
                        {
                            if (failedEndpoints != null) failedEndpoints.Add(pair.Key);
                        }
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
        }
    }
}
