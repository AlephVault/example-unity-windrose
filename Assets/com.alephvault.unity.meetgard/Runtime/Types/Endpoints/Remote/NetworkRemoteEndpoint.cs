using AlephVault.Unity.Support.Utils;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace AlephVault.Unity.Meetgard
{
    namespace Types
    {
        using AlephVault.Unity.Binary;
        using System.Collections.Concurrent;
        using System.Threading.Tasks;

        /// <summary>
        ///   <para>
        ///     A network endpoint serves for remote, non-host,
        ///     connections.
        ///   </para>
        ///   <para>
        ///     Endpoints can be told to be closed, and manage the
        ///     send and arrival of data. Sending the data can be
        ///     done in a buffered way (via "train buffers"). Most
        ///     of these operations are asynchronous in a way or
        ///     another, and event-driven. The asynchronous calls
        ///     are synchronized into the main Unity thread, however,
        ///     via the default async execution manager.
        ///   </para>
        /// </summary>
        public class NetworkRemoteEndpoint : NetworkEndpoint
        {
            // Keeps a track of all the sockets that were used to
            // instantiate an endpoint. This does not guarantee
            // preventing the same socket to be used in a different
            // king of architecture.
            private static HashSet<TcpClient> endpointSocketsInUse = new HashSet<TcpClient>();

            // Related to the internal buffering and threads.

            /// <summary>
            ///   The time to sleep, on each iteration, when no data to
            ///   read or write is present in the socket on a given
            ///   iteration.
            /// </summary>
            public readonly float IdleSleepTime;

            /// <summary>
            ///   The maximum size of each individual message to be sent.
            /// </summary>
            public readonly ushort MaxMessageSize;

            /// <summary>
            ///   The time this client waits for more data after some
            ///   message data was sent to the internal outgoing messages
            ///   buffer.
            /// </summary>
            public readonly float TrainBoardingTime;

            /// <summary>
            ///   <para>
            ///     The size of the train buffer size. This is the maximum
            ///     size of the whole buffer to send, as well. The effective
            ///     size to send will be lower, most of the times. This
            ///     value will be 6 * maxMessageSize.
            ///   </para>
            ///   <para>
            ///     Client and server should agree on this value.
            ///   </para>
            /// </summary>
            public readonly ushort TrainBufferSize;

            /// <summary>
            ///   <para>
            ///     The threshold size of the train buffer to send. When the
            ///     effective size of the train buffer size is greater than
            ///     or equal to this value, the buffer will be immediately
            ///     sent. This value will be 4 * maxMessageSize.
            ///   </para>
            ///   <para>
            ///     Client and server should agree on this value.
            ///   </para>
            /// </summary>
            public readonly ushort TrainBufferThresholdSize;

            // Related to the life-cycle and underlying objects.

            // A life-cycle thread for our socket.
            private Thread lifeCycle = null;

            // The socket, created in our life-cycle.
            private TcpClient remoteSocket = null;

            // Related to the events.

            // When a connection is established, this callback is processed.
            private Action onConnectionStart = null;

            // When a message is received, this callback is processed, passing
            // a protocol ID, a message tag, and a reader for the incoming buffer.
            private Action<ushort, ushort, ISerializable> onMessage = null;

            // When a connection is terminated, this callback is processed.
            // If the termination was not graceful, the exception that caused
            // the termination will be given. Otherwise, it will be null.
            private Action<System.Exception> onConnectionEnd = null;

            // On each arriving message, this function will be invoked to get
            // the get an object of the appropriate type to deserialize the
            // message content into.
            private Func<ushort, ushort, ISerializable> protocolMessageFactory = null;

            // Related to the messages.

            // The list of queued outgoing messages.
            private ConcurrentQueue<Tuple<ushort, ushort, ISerializable>> queuedOutgoingMessages = new ConcurrentQueue<Tuple<ushort, ushort, ISerializable>>();

            // The list of queued incoming messages.
            private ConcurrentQueue<Tuple<ushort, ushort, ISerializable>> queuedIncomingMessages = new ConcurrentQueue<Tuple<ushort, ushort, ISerializable>>();

            public NetworkRemoteEndpoint(
                TcpClient endpointSocket, Func<ushort, ushort, ISerializable> protocolMessageFactory,
                Action onConnected, Action<ushort, ushort, ISerializable> onArrival, Action<System.Exception> onDisconnected,
                ushort maxMessageSize = 1024, float trainBoardingTime = 0.75f, float idleSleepTime = 0.01f
            ) {
                if (endpointSocket == null || !endpointSocket.Connected || endpointSocketsInUse.Contains(endpointSocket))
                {
                    // This, however, does not prevent or detect the socket being used in different places.
                    // Ensure this socket is used only once, by your own means.
                    throw new ArgumentException("A unique connected socket must be passed to the endpoint construction");
                }
                if (onConnected.GetInvocationList().Length != 1)
                {
                    throw new ArgumentException("Only one handler for the onConnected event is allowed");
                }
                onConnectionStart += onConnected;
                if (onArrival.GetInvocationList().Length != 1)
                {
                    throw new ArgumentException("Only one handler for the onArrival event is allowed");
                }
                onMessage += onArrival;
                if (onDisconnected.GetInvocationList().Length != 1)
                {
                    throw new ArgumentException("Only one handler for the onDisconnected event is allowed");
                }
                onConnectionEnd += onDisconnected;

                // Prepare values related to the internal buffering.
                MaxMessageSize = Values.Clamp(512, maxMessageSize, 6144);
                TrainBoardingTime = Values.Clamp(0.5f, trainBoardingTime, 1f);
                IdleSleepTime = Values.Clamp(0.005f, idleSleepTime, 0.5f);
                TrainBufferSize = (ushort)(6 * MaxMessageSize);
                TrainBufferThresholdSize = (ushort)(4 * MaxMessageSize);

                // Prepare the settings for incoming messages.
                if (protocolMessageFactory.GetInvocationList().Length != 1)
                {
                    throw new ArgumentException("Only one handler for the getMessage callback is allowed");
                }
                this.protocolMessageFactory += protocolMessageFactory;

                // Mark the socket as in use, and also start the lifecycle.
                remoteSocket = endpointSocket;
                endpointSocketsInUse.Add(endpointSocket);
                lifeCycle = new Thread(new ThreadStart(LifeCycle));
                lifeCycle.IsBackground = true;
                lifeCycle.Start();
            }

            ~NetworkRemoteEndpoint()
            {
                endpointSocketsInUse.Remove(remoteSocket);
            }

            // Related to connection's status.

            /// <summary>
            ///   Tells whether the life-cycle is active or not. While Active, another
            ///   life-cycle (e.g. a call to <see cref="Connect(IPAddress, int)"/> or
            ///   <see cref="Connect(string, int)"/>) cannot be done.
            /// </summary>
            public override bool IsActive { get { return lifeCycle.IsAlive; } }

            /// <summary>
            ///   Tells whether the underlying socket is instantiated and connected.
            /// </summary>
            public override bool IsConnected { get { return remoteSocket.Connected; } }

            // Related to the available actions over a socket.

            /// <summary>
            ///   Closes the active connection, if any. This, actually,
            ///   tells the thread to close the connection.
            /// </summary>
            public override void Close()
            {
                if (!IsConnected)
                {
                    throw new InvalidOperationException("The socket is not connected - It cannot be closed");
                }

                remoteSocket.Close();
            }

            /// <summary>
            ///   Queues the message to be sent.
            /// </summary>
            /// <param name="protocolId">The id of protocol for this message</param>
            /// <param name="messageTag">The tag of the message being sent</param>
            /// <param name="data">The object to serialize and send</param>
            protected override async Task DoSend(ushort protocolId, ushort messageTag, ISerializable data)
            {
                queuedOutgoingMessages.Enqueue(new Tuple<ushort, ushort, ISerializable>(protocolId, messageTag, data));
            }

            // Invokes the method DoTriggerOnConnectionStart, which is
            // asynchronous in nature.
            private void TriggerOnConnectionStart()
            {
                DoTriggerOnConnectionStart();
            }

            // Triggers the onConnectionStart event into the main Unity thread.
            // This operation is done asynchronously, however.
            private async void DoTriggerOnConnectionStart()
            {
                onConnectionStart?.Invoke();
            }

            // Invokes the method DoTriggerOnConnectionEnd, which is asynchronous
            // in nature.
            private void TriggerOnConnectionEnd(System.Exception exception)
            {
                DoTriggerOnConnectionEnd(exception);
            }

            // Triggers the onConnectionEnd event into the main Unity thread.
            // This operation is done asynchronously, however.
            private async void DoTriggerOnConnectionEnd(System.Exception exception)
            {
                onConnectionEnd?.Invoke(exception);
            }

            // Invokes the method DoTriggerOnMessageEvent, which is asynchronous
            // in nature.
            private void TriggerOnMessageEvent()
            {
                DoTriggerOnMessageEvent();
            }

            // Triggers the onMessage event into the main Unity thread.
            // This operation is done asynchronously, however.
            private async void DoTriggerOnMessageEvent()
            {
                if (queuedIncomingMessages.TryDequeue(out var result))
                {
                    onMessage?.Invoke(result.Item1, result.Item2, result.Item3);
                }
            }

            // The full socket lifecycle goes here.
            private void LifeCycle()
            {
                System.Exception lifeCycleException = null;
                byte[] outgoingMessageArray = new byte[MaxMessageSize];
                byte[] incomingMessageArray = new byte[MaxMessageSize];
                try
                {
                    lifeCycleException = null;
                    // So far, remoteSocket WILL be connected.
                    TriggerOnConnectionStart();
                    // We get the stream once.
                    NetworkStream stream = remoteSocket.GetStream();
                    while (true)
                    {
                        try
                        {
                            bool inactive = true;
                            if (stream.CanRead && stream.DataAvailable)
                            {
                                Tuple<MessageHeader, ISerializable> result;
                                // protocolMessageFactory must throw an exception when
                                // the message is not understood. Such exception will
                                // blindly close the connection.
                                result = MessageUtils.ReadMessage(stream, protocolMessageFactory, outgoingMessageArray);
                                queuedIncomingMessages.Enqueue(new Tuple<ushort, ushort, ISerializable>(result.Item1.ProtocolId, result.Item1.MessageTag, result.Item2));
                                TriggerOnMessageEvent();
                                inactive = false;
                            }
                            if (stream.CanWrite && !queuedOutgoingMessages.IsEmpty)
                            {
                                while (queuedOutgoingMessages.TryDequeue(out var result)) {
                                    MessageUtils.WriteMessage(stream, result.Item1, result.Item2, result.Item3, outgoingMessageArray);
                                }
                                inactive = false;
                            }
                            if (inactive)
                            {
                                // On inactivity we sleep a while, to not hog
                                // the processor.
                                Thread.Sleep((int)(IdleSleepTime * 1000));
                            }
                        }
                        catch (InvalidOperationException)
                        {
                            // Simply return, for the socket is closed.
                            // This happened, probably, gracefully. The
                            // `finally` block will still do the cleanup.
                            return;
                        }
                    }
                }
                catch (System.Exception e)
                {
                    // Keep the exception, and return from the
                    // whole thread execution.
                    lifeCycleException = e;
                }
                finally
                {
                    // On closure, if the socket is connected,
                    // it will be closed. Then, it will be
                    // disposed and unassigned.
                    if (remoteSocket != null)
                    {
                        if (remoteSocket.Connected) remoteSocket.Close();
                        remoteSocket.Dispose();
                    }
                    // Also, clear the thread reference.
                    lifeCycle = null;
                    // Finally, trigger the disconnected event.
                    TriggerOnConnectionEnd(lifeCycleException);
                }
            }
        }
    }
}
