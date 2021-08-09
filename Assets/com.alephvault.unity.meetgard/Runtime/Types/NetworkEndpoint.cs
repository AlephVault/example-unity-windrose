using AlephVault.Unity.Support.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace AlephVault.Unity.Meetgard
{
    namespace Types
    {
        using AlephVault.Unity.Binary;
        using System.IO;
        using System.Threading.Tasks;

        /// <summary>
        ///   <para>
        ///     A network endpoint serves both for (true, non-host)
        ///     clients and per-{[true, non-host] connection} server
        ///     handlers.
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
        public class NetworkEndpoint
        {
            // Keeps a track of all the sockets that were used to
            // instantiate an endpoint. This does not guarantee
            // preventing the same socket to be used in a different
            // king of architecture.
            private static HashSet<TcpClient> endpointSocketsInUse = new HashSet<TcpClient>();

            // This exception class has internal purposes
            // only and will never be exported. It is meant
            // to notify a graceful client shutdown.
            private class GracefulShutdown : Exception
            {
                public GracefulShutdown() : base() { }
            }

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

            // The current boarding time this client has been waiting
            // for additional data after some data was sent to the internal
            // outgoing messages buffer.
            private float currentBoardingTime = 0;

            // The train buffer.
            private Buffer trainBuffer;

            // A writer for the train buffer.
            private Writer trainWriter;

            // Tells whether the train buffer is being sent right now
            // or not (if not, it means that new messages can be buffered
            // right now; otherwise, it means that new messages must wait
            // to be sent).
            private bool trainSending = false;

            // A life-cycle thread for our socket.
            private Thread lifeCycle = null;

            // The socket, created in our life-cycle.
            private TcpClient remoteSocket = null;

            // This event allows us to synchronize sending the newly read data
            // and its handling: if data is still being handled, then this object
            // will block the lifecycle thread until this behaviour is ready
            // to attend more incoming data.
            private ManualResetEvent incomingDataToll = new ManualResetEvent(false);

            // When a connection is established, this callback is processed.
            private Action onConnectionStart = null;

            // When a message is received, this callback is processed, passing
            // a protocol ID, a message tag, and a reader for the incoming buffer.
            private Action<ushort, ushort, Reader> onMessage = null;

            // When a connection is terminated, this callback is processed.
            // If the termination was not graceful, the exception that caused
            // the termination will be given. Otherwise, it will be null.
            private Action<Exception> onConnectionEnd = null;

            public NetworkEndpoint(
                TcpClient endpointSocket,
                Action onConnected, Action<ushort, ushort, Reader> onArrival, Action<Exception> onDisconnected,
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

                MaxMessageSize = Values.Clamp(512, maxMessageSize, 6144);
                TrainBoardingTime = Values.Clamp(0.5f, trainBoardingTime, 1f);
                IdleSleepTime = Values.Clamp(0.005f, idleSleepTime, 0.5f);
                TrainBufferSize = (ushort)(6 * MaxMessageSize);
                TrainBufferThresholdSize = (ushort)(4 * MaxMessageSize);
                remoteSocket = endpointSocket;
                endpointSocketsInUse.Add(endpointSocket);
                // Run a life-cycle thread.
                lifeCycle = new Thread(new ThreadStart(LifeCycle));
                lifeCycle.IsBackground = true;
                lifeCycle.Start();
            }

            /// <summary>
            ///   Tells whether the life-cycle is active or not. While Active, another
            ///   life-cycle (e.g. a call to <see cref="Connect(IPAddress, int)"/> or
            ///   <see cref="Connect(string, int)"/>) cannot be done.
            /// </summary>
            public bool Active { get { return lifeCycle != null && lifeCycle.IsAlive; } }

            /// <summary>
            ///   Tells whether the underlying socket is instantiated and connected.
            /// </summary>
            public bool Connected { get { return remoteSocket != null && remoteSocket.Connected; } }

            /// <summary>
            ///   Closes the active connection, if any. This, actually,
            ///   tells the thread to close the connection.
            /// </summary>
            public void Close()
            {
                if (!Connected)
                {
                    throw new InvalidOperationException("The socket is not connected - It cannot be closed");
                }

                remoteSocket.Close();
            }

            /// <summary>
            ///   Sends a stream through the network. This function is asynchronous
            ///   and will wait until no other messages are pending to be sent.
            /// </summary>
            /// <param name="protocolID">The id of protocol for this message</param>
            /// <param name="messageTag">The tag of the message being sent</param>
            /// <param name="input">The input stream</param>
            public Task Send(ushort protocolID, ushort messageTag, Stream input)
            {
                if (!Connected)
                {
                    throw new InvalidOperationException("The socket is not connected - No data can be sent");
                }

                if (input == null)
                {
                    throw new ArgumentNullException("stream");
                }

                if (input.Length > MaxMessageSize)
                {
                    throw new ArgumentException($"The size of the stream ({input.Length}) is greater than the allowed message size ({MaxMessageSize})");
                }

                return DoSend(protocolID, messageTag, input);
            }

            private async Task DoSend(ushort protocolID, ushort messageTag, Stream input)
            {
                while (trainSending) await Tasks.Blink();
                // The buffer is ready to be written, so the message is now
                // being written and, then, it will be determined whether
                // it must send or not the whole buffer.
                trainWriter.WriteUInt16(protocolID);
                trainWriter.WriteUInt16(messageTag);
                trainWriter.WriteUInt16((ushort)input.Length);
                trainWriter.ReadAndWrite(new Reader(input), input.Length);
                if (trainBuffer.Length >= TrainBufferThresholdSize)
                {
                    // The buffer is now marked to be uploaded.
                    // The life-cycle thread will handle this.
                    trainSending = true;
                }
                else
                {
                    // We delay the launch of trainSending.
                    DelayTrainSend();                    
                }
            }

            // We delay marking the train ready to be sent. Once there, the
            // thread will understand the message and send the data. After
            // the data is sent, the thread will clear that flag.
            private async void DelayTrainSend()
            {
                float time = 0;
                while(time < TrainBoardingTime)
                {
                    await Tasks.Blink();
                    time += Time.unscaledDeltaTime;
                }
                trainSending = true;
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
            private void TriggerOnConnectionEnd(Exception exception)
            {
                DoTriggerOnConnectionEnd(exception);
            }

            // Triggers the onConnectionEnd event into the main Unity thread.
            // This operation is done asynchronously, however.
            private async void DoTriggerOnConnectionEnd(Exception exception)
            {
                onConnectionEnd?.Invoke(exception);
            }

            // Invokes the method DoTriggerOnMessageEvent, which is asynchronous
            // in nature, but after resetting the toll.
            private async void TriggerOnMessageEvent(Func<Tuple<ushort, ushort, Reader, Buffer>> fillIncomingMessageBuffer)
            {
                incomingDataToll.Reset();
                DoTriggerOnMessageEvent(fillIncomingMessageBuffer);
            }

            // Triggers the onMessage event into the main Unity thread.
            // This operation is done asynchronously, however.
            private async void DoTriggerOnMessageEvent(Func<Tuple<ushort, ushort, Reader, Buffer>> fillIncomingMessageBuffer)
            {
                try
                {
                    // Filling the messageContentUnderlyingBuffer from the network input data.
                    Tuple<ushort, ushort, Reader, Buffer> result = fillIncomingMessageBuffer();
                    // This event cannot be processed asynchronously. This is due to the fact that the
                    // buffer will be released after the event triggering. If the triggering was instead
                    // of an asynchronous nature, the buffer would be empty wuen trying to process it.
                    onMessage?.Invoke(result.Item1, result.Item2, result.Item3);
                    if (result.Item4.Length > 0)
                    {
                        Debug.LogWarning($"After processing a NetworkEndpoint incoming message, {result.Item4.Length} remained, and were discarded - unexhausted incoming buffers might be a sign of user implementation issues");
                        new Writer(Stream.Null).ReadAndWrite(result.Item3, result.Item4.Length);
                    }
                }
                finally
                {
                    // We remove the mark of incoming data and also we
                    // set the event so the lifecycle can read anything
                    // as normal.
                    incomingDataToll.Set();
                }
            }

            // The full socket lifecycle goes here.
            private void LifeCycle()
            {
                Exception lifecycleException = null;
                Buffer incomingMessageBuffer = null;
                Writer incomingMessageWriter;
                Reader incomingMessageReader;
                try
                {
                    trainBuffer = new Buffer(TrainBufferSize);
                    trainWriter = new Writer(trainBuffer);
                    lifecycleException = null;
                    incomingMessageBuffer = new Buffer(MaxMessageSize);
                    incomingMessageWriter = new Writer(incomingMessageBuffer);
                    incomingMessageReader = new Reader(incomingMessageBuffer);
                    // So far, remoteSocket WILL be connected.
                    TriggerOnConnectionStart();
                    while (true)
                    {
                        try
                        {
                            using (NetworkStream stream = remoteSocket.GetStream())
                            {
                                bool inactive = true;
                                if (stream.CanRead && stream.DataAvailable)
                                {
                                    // Before reading anything, we must ensure we're allowed
                                    // to read or we must wait since another read is in progress
                                    // for this thread.
                                    incomingDataToll.WaitOne();
                                    // Now we mark, again, as the buffer not being allowed to be used.
                                    // Also, we mark incoming data as present, so the behaviour side can process it.
                                    TriggerOnMessageEvent(() =>
                                    {
                                        // This is the callback function to read the buffers.
                                        // The first thing to do is read the header.
                                        Reader reader = new Reader(stream);
                                        ushort protocolId = reader.ReadUInt16();
                                        ushort messageTag = reader.ReadUInt16();
                                        ushort messageSize = reader.ReadUInt16();
                                        if (messageSize >= MaxMessageSize)
                                        {
                                            throw new MessageOverflowException($"A message was received telling it had {messageSize} bytes, which is more than the {MaxMessageSize} bytes allowed per message");
                                        }
                                        // Read the whole buffer (i.e. wait for messageSize bytes, and read into a new buffer).
                                        // Also set the incoming metadata variables accordingly.
                                        incomingMessageWriter.ReadAndWrite(reader, messageSize);
                                        return new Tuple<ushort, ushort, Reader, Buffer>(protocolId, messageTag, incomingMessageReader, incomingMessageBuffer);
                                    });
                                    inactive = false;
                                }
                                if (stream.CanWrite && trainSending)
                                {
                                    if (trainBuffer.Length > 0) new Writer(stream).ReadAndWrite(new Reader(trainBuffer), trainBuffer.Length);
                                    trainSending = false;
                                    inactive = false;
                                }
                                if (inactive)
                                {
                                    // On inactivity we sleep a while, to not hog
                                    // the processor.
                                    Thread.Sleep((int)(IdleSleepTime * 1000));
                                }
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
                catch (Exception e)
                {
                    // Keep the exception, and return from the
                    // whole thread execution.
                    lifecycleException = e;
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
                        remoteSocket = null;
                    }
                    // Also, clear the thread reference.
                    lifeCycle = null;
                    // Also, if by chance there is a buffer to
                    // send that was not by any reason, then
                    // it must be cleared (and any pending).
                    trainSending = false;
                    trainBuffer.Dispose();
                    trainBuffer = null;
                    trainWriter = null;
                    // We also dispose the incoming message buffer
                    // as well (with all the related variables).
                    incomingMessageBuffer?.Dispose();
                    // Finally, trigger the disconnected event.
                    TriggerOnConnectionEnd(lifecycleException);
                }
            }
        }
    }
}
