using AlephVault.Unity.Binary;
using AlephVault.Unity.Meetgard.Types;
using AlephVault.Unity.Support.Utils;
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

            private void Awake()
            {
                maxMessageSize = Values.Clamp(512, maxMessageSize, 6144);
                idleSleepTime = Values.Clamp(0.005f, idleSleepTime, 0.5f);
                SetupClientProtocols();
            }

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
            /// <typeparam name="T">The type of the message being sent</typeparam>
            /// <param name="clientId">The id of the client</param>
            /// <param name="protocol">The protocol for this message. It must be an already attached component</param>
            /// <param name="message">The message (as it was registered) being sent</param>
            /// <param name="content">The message content</param>
            /// <returns>Whether the endpoint existed or not (if true, the message was sent)</returns>
            public async Task<bool> Send<T>(IProtocolServerSide protocol, string message, ulong clientId, T content) where T : ISerializable
            {
                if (protocol == null)
                {
                    throw new ArgumentNullException("protocol");
                }

                if (!IsRunning)
                {
                    throw new InvalidOperationException("The server is not running - cannot send any message");
                }

                ushort protocolId = GetProtocolId(protocol);
                ushort messageTag;
                Type expectedType;
                try
                {
                    messageTag = GetOutgoingMessageTag(protocolId, message);
                    expectedType = GetOutgoingMessageType(protocolId, messageTag);
                }
                catch (UnexpectedMessageException e)
                {
                    // Reformatting the exception.
                    throw new UnexpectedMessageException($"Unexpected outgoing protocol/message: ({protocol.GetType().FullName}, {message})", e);
                }

                if (content.GetType() != expectedType)
                {
                    throw new OutgoingMessageTypeMismatchException($"Outgoing message ({protocol.GetType().FullName}, {message}) was attempted with type {content.GetType().FullName} when {expectedType.FullName} was expected");
                }

                if (endpointById.TryGetValue(clientId, out NetworkEndpoint endpoint))
                {
                    await endpoint.Send(protocolId, messageTag, content);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            /// <summary>
            ///   Sends a message through the network.
            /// </summary>
            /// <typeparam name="T">The type of the message being sent</typeparam>
            /// <typeparam name="ProtocolType">The protocol type for this message. One instance of it must be an already attached component</param>
            /// <param name="clientId">The id of the client</param>
            /// <param name="message">The message (as it was registered) being sent</param>
            /// <param name="content">The message content</param>
            /// <returns>Whether the endpoint existed or not (if true, the message was sent)</returns>
            public Task<bool> Send<ProtocolType, T>(string message, ulong clientId, T content) where ProtocolType : IProtocolServerSide where T : ISerializable
            {
                ProtocolType protocol = GetComponent<ProtocolType>();
                if (protocol == null)
                {
                    throw new UnknownProtocolException($"This object does not have a protocol of type {protocol.GetType().FullName} attached to it");
                }
                else
                {
                    return Send(protocol, message, clientId, content);
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
            /// <typeparam name="T">The type of the message being sent</typeparam>
            /// <param name="clientIds">The ids to send the same message - use null to specify ALL the available ids</param>
            /// <param name="protocol">The protocol for this message. It must be an already attached component</param>
            /// <param name="message">The message (as it was registered) being sent</param>
            /// <param name="content">The message content</param>
            /// <param name="failedEndpoints">The output list of the endpoints that are not found or raised an error on send</param>
            public async Task Broadcast<T>(ulong[] clientIds, IProtocolServerSide protocol, string message, T content, HashSet<ulong> failedEndpoints) where T : ISerializable
            {
                if (protocol == null)
                {
                    throw new ArgumentNullException("protocol");
                }

                if (!IsRunning)
                {
                    throw new InvalidOperationException("The server is not running - cannot send any message");
                }

                ushort protocolId = GetProtocolId(protocol);
                ushort messageTag;
                Type expectedType;
                try
                {
                    messageTag = GetOutgoingMessageTag(protocolId, message);
                    expectedType = GetOutgoingMessageType(protocolId, messageTag);
                }
                catch (UnexpectedMessageException e)
                {
                    // Reformatting the exception.
                    throw new UnexpectedMessageException($"Unexpected outgoing protocol/message: ({protocol.GetType().FullName}, {message})", e);
                }

                if (content.GetType() != expectedType)
                {
                    throw new OutgoingMessageTypeMismatchException($"Outgoing message ({protocol.GetType().FullName}, {message}) was attempted with type {content.GetType().FullName} when {expectedType.FullName} was expected");
                }

                // Now, with everything ready, the send can be done.

                // Clearing the target set is the first thing to do.
                failedEndpoints?.Clear();

                if (clientIds == null)
                {
                    // Only the specified endpoints will be iterated.
                    foreach (ulong clientId in clientIds)
                    {
                        if (endpointById.TryGetValue(clientId, out NetworkEndpoint endpoint))
                        {
                            try
                            {
                                await endpoint.Send(protocolId, messageTag, content);
                            }
                            catch
                            {
                                failedEndpoints?.Add(clientId);
                            }
                        }
                        else
                        {
                            failedEndpoints?.Add(clientId);
                        }
                    }
                }
                else
                {
                    // All of the endpoints will be iterated.
                    foreach(KeyValuePair<ulong, NetworkEndpoint> pair in endpointById.ToArray())
                    {
                        try
                        {
                            await pair.Value.Send(protocolId, messageTag, content);
                        }
                        catch
                        {
                            failedEndpoints?.Add(pair.Key);
                        }
                    }
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
            /// <typeparam name="T">The type of the message being sent</typeparam>
            /// <typeparam name="ProtocolType">The protocol type for this message. One instance of it must be an already attached component</param>
            /// <param name="clientIds">The ids to send the same message - use null to specify ALL the available ids</param>
            /// <param name="message">The message (as it was registered) being sent</param>
            /// <param name="content">The message content</param>
            /// <param name="failedEndpoints">The output list of the endpoints that are not found or raised an error on send</param>
            public Task Broadcast<ProtocolType, T>(ulong[] clientIds, string message, T content, HashSet<ulong> failedEndpoints) where ProtocolType : IProtocolServerSide where T : ISerializable
            {
                ProtocolType protocol = GetComponent<ProtocolType>();
                if (protocol == null)
                {
                    throw new UnknownProtocolException($"This object does not have a protocol of type {protocol.GetType().FullName} attached to it");
                }
                else
                {
                    return Broadcast(clientIds, protocol, message, content, failedEndpoints);
                }
            }

            /// <summary>
            ///   Closes a registered endpoint by its id.
            /// </summary>
            /// <param name="clientId">The id of the client</param>
            /// <returns>Whether the endpoint existed or not (if true, it was also closed)</returns>
            public bool Close(ulong clientId)
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
