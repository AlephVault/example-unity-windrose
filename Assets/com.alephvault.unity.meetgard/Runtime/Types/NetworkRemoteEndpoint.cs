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

            // The id of the next buffer filling request (inside a
            // call to Send()) that will be processed.
            private ulong nextFillBufferRequestToQueue = 1;

            // Queue of currently waiting buffer filling requests (
            // inside Send() calls).
            private List<ulong> fillBufferRequestsQueue = new List<ulong>();

            // A mutex to increment the nextFillBufferRequestToQueue and also queuing it.
            private SemaphoreSlim fillBufferRequestsMutex = new SemaphoreSlim(1, 1);

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

            // The train buffer.
            private Buffer trainBuffer;

            // A writer for the train buffer.
            private Writer trainWriter;
            
            // Tells whether a call to DelayedTrainSend is in progress.
            // This, to avoid delaying twice.
            private bool delayedTrainSendRunning = false;

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
            private Action<System.Exception> onConnectionEnd = null;

            public NetworkRemoteEndpoint(
                TcpClient endpointSocket,
                Action onConnected, Action<ushort, ushort, Reader> onArrival, Action<System.Exception> onDisconnected,
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
                // Set the buffer event, so reads are allowed.
                incomingDataToll.Set();
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
            public override bool IsActive { get { return lifeCycle.IsAlive; } }

            /// <summary>
            ///   Tells whether the underlying socket is instantiated and connected.
            /// </summary>
            public override bool IsConnected { get { return remoteSocket.Connected; } }

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
            ///   Sends a stream through the network. This function is asynchronous
            ///   and will wait until no other messages are pending to be sent.
            /// </summary>
            /// <param name="protocolId">The id of protocol for this message</param>
            /// <param name="messageTag">The tag of the message being sent</param>
            /// <param name="input">The input stream</param>
            public override async Task Send(ushort protocolId, ushort messageTag, Stream input)
            {
                if (!IsConnected)
                {
                    throw new InvalidOperationException("The socket is not connected - No data can be sent");
                }

                if (input.Length > MaxMessageSize)
                {
                    throw new ArgumentException($"The size of the stream ({input.Length}) is greater than the allowed message size ({MaxMessageSize})");
                }

                await DoSend(protocolId, messageTag, input);
            }

            protected async Task DoSend(ushort protocolId, ushort messageTag, Stream input)
            {
                if (input == null)
                {
                    throw new ArgumentNullException("stream");
                }

                // Atomically getting the next id to use for send, and queuing it.
                // Notice how we intentionally avoid using sendId == 0.
                await fillBufferRequestsMutex.WaitAsync();
                ulong sendId = nextFillBufferRequestToQueue;
                if (nextFillBufferRequestToQueue == ulong.MaxValue)
                {
                    nextFillBufferRequestToQueue = 1;
                }
                else
                {
                    nextFillBufferRequestToQueue += 1;
                }
                fillBufferRequestsQueue.Add(sendId);
                fillBufferRequestsMutex.Release();

                // Then, waiting until we find our id as the head of the queue.
                while (fillBufferRequestsQueue[0] != sendId) await Tasks.Blink();
                // The buffer is ready to be written, so the message is now
                // being written and, then, it will be determined whether
                // it must send or not the whole buffer.
                trainWriter.WriteUInt16(protocolId);
                trainWriter.WriteUInt16(messageTag);
                trainWriter.WriteUInt16((ushort)input.Length);
                trainWriter.ReadAndWrite(new Reader(input), input.Length);
                if (trainBuffer.Length >= TrainBufferThresholdSize)
                {
                    // Here, we do nothing. The server will understand that
                    // there is a value on fillBufferRequestsQueue and will
                    // send the buffer. After that, the server will release
                    // by calling fillBufferRequestsQueue.RemoveAt(0);
                }
                else
                {
                    // So far, we can remove our buffer interaction
                    // id, since we're done with this request.
                    fillBufferRequestsQueue.Remove(sendId);
                    // However, we tell that the buffer will be used
                    // and sent... later.
                    DelayTrainSend();                    
                }
            }

            // We delay marking the train ready to be sent. Once there, the
            // thread will understand the message and send the data. After
            // the data is sent, the thread will clear that flag.
            private async void DelayTrainSend()
            {
                if (delayedTrainSendRunning) return;
                try
                {
                    delayedTrainSendRunning = true;
                    float time = 0;
                    while(time < TrainBoardingTime)
                    {
                        await Tasks.Blink();
                        time += Time.unscaledDeltaTime;
                    }

                    // Atomically adding the send request 0, which is a magical
                    // token telling the server to automatically send any
                    // pending buffer.
                    await fillBufferRequestsMutex.WaitAsync();
                    fillBufferRequestsQueue.Add(0);
                    fillBufferRequestsMutex.Release();
                }
                finally
                {
                    delayedTrainSendRunning = false;
                }
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
            // in nature, but after resetting the toll.
            private void TriggerOnMessageEvent(ushort protocolId, ushort messageTag, Reader messageReader, Buffer messageContent)
            {
                DoTriggerOnMessageEvent(protocolId, messageTag, messageReader, messageContent);
            }

            // Triggers the onMessage event into the main Unity thread.
            // This operation is done asynchronously, however.
            private async void DoTriggerOnMessageEvent(ushort protocolId, ushort messageTag, Reader messageReader, Buffer messageContent)
            {
                try
                {
                    // Filling the messageContentUnderlyingBuffer from the network input data.
                    // An exception will be thrown here if fetching the result involves an
                    // attack on message size.
                    Debug.Log($"Processing an incoming message ({protocolId}.{messageTag})");
                    // Now, the message is to be processed.
                    onMessage?.Invoke(protocolId, messageTag, messageReader);
                }
                finally
                {
                    // Releasing the buffer, if any. But also giving a warning.
                    if (messageContent != null && messageContent.Length > 0)
                    {
                        Debug.LogWarning($"After processing a NetworkEndpoint incoming message, {messageContent.Length} remained, and were discarded - unexhausted incoming buffers might be a sign of user implementation issues");
                        new Writer(Stream.Null).ReadAndWrite(messageReader, messageContent.Length);
                    }
                    // We remove the mark of incoming data and also we
                    // set the event so the lifecycle can read anything
                    // as normal.
                    incomingDataToll.Set();
                }
            }

            // Forces the reading of a buffer, to always expect at least N bytes
            private void ReadUntil(Stream sourceBuffer, Stream targetBuffer, int howMuch)
            {
                // TODO We need to download all of the incoming buffer into an
                // TODO intermediary buffer (of size: MaxMessageSize + 6) and
                // TODO then we move forward.
            }

            // The full socket lifecycle goes here.
            private void LifeCycle()
            {
                System.Exception lifeCycleException = null;
                Buffer incomingMessageBuffer = null;
                Writer incomingMessageWriter;
                Reader incomingMessageReader;
                try
                {
                    trainBuffer = new Buffer(TrainBufferSize);
                    trainWriter = new Writer(trainBuffer);
                    lifeCycleException = null;
                    incomingMessageBuffer = new Buffer(MaxMessageSize);
                    incomingMessageWriter = new Writer(incomingMessageBuffer);
                    incomingMessageReader = new Reader(incomingMessageBuffer);
                    // So far, remoteSocket WILL be connected.
                    TriggerOnConnectionStart();
                    while (true)
                    {
                        try
                        {
                            NetworkStream stream = remoteSocket.GetStream();
                            bool inactive = true;
                            if (stream.CanRead && stream.DataAvailable)
                            {
                                // Before reading anything, we must ensure we're allowed
                                // to read or we must wait since another read is in progress
                                // for this thread. We then lock the allowance.
                                incomingDataToll.WaitOne();
                                incomingDataToll.Reset();
                                // First, we read the message header: rewinding, reading 6, rewinding.
                                incomingMessageBuffer.Seek(0, SeekOrigin.Begin);
                                ReadUntil(stream, incomingMessageBuffer, 6);
                                incomingMessageBuffer.Seek(0, SeekOrigin.Begin);
                                // Then we read the message header from the buffer and check for
                                // a valid, allowed, size.
                                Debug.Log("Fill Incoming Message Buffer :: Create reader");
                                Debug.Log("Fill Incoming Message Buffer :: Read Protocol ID");
                                ushort protocolId = incomingMessageReader.ReadUInt16();
                                Debug.Log($"Protocol ID is: {protocolId}");
                                Debug.Log("Fill Incoming Message Buffer :: Read Message Tag");
                                ushort messageTag = incomingMessageReader.ReadUInt16();
                                Debug.Log($"Message Tag is: {messageTag}");
                                Debug.Log("Fill Incoming Message Buffer :: Read Message Size");
                                ushort messageSize = incomingMessageReader.ReadUInt16();
                                Debug.Log($"Message Size is: {messageSize}");
                                Debug.Log("Fill Incoming Message Buffer :: Check Message Size");
                                if (messageSize >= MaxMessageSize)
                                {
                                    Debug.Log($"Bad size! {messageSize} >= {MaxMessageSize}");
                                    throw new MessageOverflowException($"A message was received telling it had {messageSize} bytes, which is more than the {MaxMessageSize} bytes allowed per message");
                                }
                                // Then we read the message content. Right now the buffer will
                                // be at position=6, so we rewind and we will read on it, instead.
                                Debug.Log("Fill Incoming Message Buffer :: Write into incoming data");
                                incomingMessageBuffer.Seek(0, SeekOrigin.Begin);
                                ReadUntil(stream, incomingMessageBuffer, messageSize);
                                // Triggering the event, asynchronously.
                                TriggerOnMessageEvent(protocolId, messageTag, incomingMessageReader, incomingMessageBuffer);
                                inactive = false;
                            }
                            if (stream.CanWrite)
                            {
                                try
                                {
                                    fillBufferRequestsMutex.Wait();
                                    bool hasWaiting = fillBufferRequestsQueue.Count > 0;
                                    if (hasWaiting)
                                    {
                                        try
                                        {
                                            Debug.Log($"Sending data ({trainBuffer.Length} bytes)...");
                                            if (trainBuffer.Length > 0) new Writer(stream).ReadAndWrite(new Reader(trainBuffer), trainBuffer.Length);
                                            // Clearing the train buffer after sending is needed!
                                            trainBuffer.Seek(0, SeekOrigin.Begin);
                                            Debug.Log("Data sent.");
                                        }
                                        finally
                                        {
                                            fillBufferRequestsQueue.RemoveAt(0);
                                            inactive = false;
                                        }
                                    }
                                }
                                finally
                                {
                                    fillBufferRequestsMutex.Release();
                                }
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
                    // Also, if by chance there is a buffer to
                    // send that was not by any reason, then
                    // it must be cleared (and any pending).
                    trainBuffer.Dispose();
                    trainBuffer = null;
                    trainWriter = null;
                    // We also dispose the incoming message buffer
                    // as well (with all the related variables).
                    incomingMessageBuffer?.Dispose();
                    // Finally, trigger the disconnected event.
                    TriggerOnConnectionEnd(lifeCycleException);
                }
            }
        }
    }
}
