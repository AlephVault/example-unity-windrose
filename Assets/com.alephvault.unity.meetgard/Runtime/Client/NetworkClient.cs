using AlephVault.Unity.Support.Utils;
using System;
using System.IO;
using UnityEngine;

namespace AlephVault.Unity.Meetgard
{
    namespace Client
    {
        using AlephVault.Unity.Binary;
        using AlephVault.Unity.Meetgard.Types;
        using System.Net;
        using System.Net.Sockets;
        using System.Threading;
        using System.Threading.Tasks;

        public class NetworkClient : MonoBehaviour
        {
            // This exception class has internal purposes
            // only and will never be exported. It is meant
            // to notify a graceful client shutdown.
            private class GracefulShutdown : Exception
            {
                public GracefulShutdown() : base() {}
            }

            /// <summary>
            ///   The time to sleep, on each iteration, when no data to
            ///   read or write is present in the socket on a given
            ///   iteration.
            /// </summary>
            [SerializeField]
            private float idleSleepTime = 0.01f;

            /// <summary>
            ///   The maximum size of each individual message to be sent.
            /// </summary>
            [SerializeField]
            private ushort maxMessageSize = 1024;

            /// <summary>
            ///   The time this client waits for more data after some
            ///   message data was sent to the internal outgoing messages
            ///   buffer.
            /// </summary>
            [SerializeField]
            private float trainBoardingTime = 0.75f;

            // The size of the train buffer size. This is the maximum
            // size of the whole buffer to send, as well. The effective
            // size to send will be lower, most of the times. This
            // value will be 6 * maxMessageSize.
            private ushort trainBufferSize = 6144;

            // The threshold size of the train buffer to send. When the
            // effective size of the train buffer size is greater than
            // or equal to this value, the buffer will be immediately
            // sent. This value will be 4 * maxMessageSize.
            private ushort trainBufferThresholdSize = 4096;

            // Tells whether the socket was connected in the last frame
            // or instead was disconnected. Useful to trigger the
            // OnConnected event accordingly.
            private bool wasConnected = false;

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

            // The exception triggered by the socket inside the life-cycle thread.
            private Exception lifecycleException = null;

            // The socket, created in our life-cycle.
            private TcpClient clientSocket = null;

            // This event allows us to synchronize sending the newly read data
            // and its handling: if data is still being handled, then this object
            // will block the lifecycle thread until this behaviour is ready
            // to attend more incoming data.
            private ManualResetEvent incomingDataToll = new ManualResetEvent(false);

            // Tells whether an incoming message is still present or not.
            private bool isIncomingMessagePresent = false;

            // The incoming message protocol id, if isIncomingMessagePresent == true.
            private ushort incomingMessageProtocolID = 0;

            // The incoming message tag, if isIncomingMessagePresent == true.
            private ushort incomingMessageTag = 0;

            // The incoming message buffer, if isIncomingMessagePresent == true.
            private Buffer incomingMessageBuffer = null;

            // A writer for the incoming message buffer, used by the life-cycle thread.
            private Writer incomingMessageWriter = null;

            // A reader for the incoming message buffer, used by the event handler.
            // Please note: only ONE event handler should make use of this reader.
            // Also note: such event handler should exhaust the buffer!
            private Reader incomingMessageReader = null;

            /// <summary>
            ///   This event is triggered when a new message arrives.
            ///   PLEASE NOTE: ONLY ONE HANDLER SHOULD HANDLE THE INCOMING MESSAGE, AND IT
            ///   SHOULD EXHAUST THE BUFFER COMPLETELY. Ideally, an `async void` function
            ///   should be accepted, which parses the incoming message protocol-wise.
            /// </summary>
            public event Action<ushort, ushort, Reader> OnMessage = null;

            /// <summary>
            ///   This event is triggered on successful connection.
            /// </summary>
            public event Action OnConnected = null;

            /// <summary>
            ///   This event is triggered on disconnection. An Exception argument tells
            ///   whether the disconnection was graceful or not: by having a null
            ///   value, it was a graceful termination.
            /// </summary>
            public event Action<Exception> OnDisconnected = null;

            /// <summary>
            ///   Tells whether the life-cycle is active or not. While Active, another
            ///   life-cycle (e.g. a call to <see cref="Connect(IPAddress, int)"/> or
            ///   <see cref="Connect(string, int)"/>) cannot be done.
            /// </summary>
            public bool Active { get { return lifeCycle != null && lifeCycle.IsAlive; } }

            /// <summary>
            ///   Tells whether the underlying socket is instantiated and connected.
            /// </summary>
            public bool Connected { get { return clientSocket != null && clientSocket.Connected; } }
            
            private void Awake()
            {
                maxMessageSize = Values.Clamp(512, maxMessageSize, 6144);
                trainBufferSize = (ushort)(6 * maxMessageSize);
                trainBufferThresholdSize = (ushort)(4 * maxMessageSize);
            }

            private void Update()
            {
                // Process the successful connection event.
                if (!wasConnected && Connected)
                {
                    OnConnected?.Invoke();
                }

                // Process any incoming message.
                if (isIncomingMessagePresent)
                {
                    try
                    {
                        OnMessage?.Invoke(incomingMessageProtocolID, incomingMessageTag, incomingMessageReader);
                        if (incomingMessageBuffer.Length > 0)
                        {
                            Debug.LogWarning($"After processing a NetworkClient incoming message, {incomingMessageBuffer.Length} remained, and were discarded - unexhausted incoming buffers might be a sign of user implementation issues");
                            new Writer(Stream.Null).ReadAndWrite(incomingMessageReader, incomingMessageBuffer.Length);
                        }
                    }
                    finally
                    {
                        // We remove the mark of incoming data and also we
                        // set the event so the lifecycle can read anything
                        // as normal.
                        isIncomingMessagePresent = false;
                        incomingDataToll.Set();
                    }
                }

                // Process any outgoing message. This only marks, in case of
                // having available data for a considerable time, the need
                // of sending the pending data.
                if (trainBuffer != null && trainBuffer.Length > 0)
                {
                    currentBoardingTime += Time.unscaledTime;
                    if (currentBoardingTime >= trainBoardingTime)
                    {
                        trainSending = true;
                    }
                }

                // Trigger this event with any exception thrown inside the life-cycle thread.
                // Exceptions in the life-cycle tell the connection to terminate. One of those
                // exception is GracefulShutdown, which is not an exception in the "error"
                // sense but just a notification that the connection was gracefully stopped
                // by either side. A graceful exception is wiped out and becomes null regarding
                // the OnDisconnected event.
                if (lifecycleException != null)
                {
                    Exception inner = lifecycleException;
                    lifecycleException = null;
                    if (inner is GracefulShutdown) inner = null;
                    OnDisconnected?.Invoke(inner);
                }

                // Update the status of wasConnected.
                wasConnected = Connected;
            }

            private void OnDestroy()
            {
                if (clientSocket != null && clientSocket.Connected)
                {
                    Close();
                }
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

                if (input.Length > maxMessageSize)
                {
                    throw new ArgumentException($"The size of the stream ({input.Length}) is greater than the allowed message size ({maxMessageSize})");
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
                if (trainBuffer.Length >= trainBufferThresholdSize)
                {
                    // The buffer is now marked to be uploaded.
                    // The life-cycle thread will handle this.
                    trainSending = true;
                }
            }

            /// <summary>
            ///   Connects to a specific address/port pair.
            /// </summary>
            /// <param name="address">Any IPv4 or IPv6 valid address</param>
            /// <param name="port">Any port nuber (in the TCP range)</param>
            public void Connect(IPAddress address, int port)
            {
                Connect(address.ToString(), port);
            }

            /// <summary>
            ///   Connects to a specific address/port pair.
            /// </summary>
            /// <param name="address">Any IPv4 or IPv6 valid address</param>
            /// <param name="port">Any port nuber (in the TCP range)</param>
            public void Connect(string address, int port)
            {
                if (Active)
                {
                    throw new InvalidOperationException("The socket is already connected - It cannot be connected again");
                }

                // Run a life-cycle thread.
                lifeCycle = new Thread(new ThreadStart(() =>
                {
                    LifeCycle(address, port);
                }));
                lifeCycle.IsBackground = true;
                lifeCycle.Start();
            }

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

                clientSocket.Close();
            }

            // The full socket lifecycle goes here.
            private void LifeCycle(string address, int port)
            {
                try
                {
                    trainBuffer = new Buffer(trainBufferSize);
                    trainWriter = new Writer(trainBuffer);
                    incomingMessageBuffer = new Buffer(maxMessageSize);
                    incomingMessageWriter = new Writer(incomingMessageBuffer);
                    incomingMessageReader = new Reader(incomingMessageBuffer);
                    clientSocket = new TcpClient(address, port);
                    while(true)
                    {
                        try
                        {
                            using(NetworkStream stream = clientSocket.GetStream())
                            {
                                bool inactive = true;
                                if (stream.CanRead && stream.DataAvailable)
                                {
                                    // We can safely read incoming metadata.
                                    // Also, check whether the max message
                                    // size is safe or not.
                                    Reader reader = new Reader(stream);
                                    ushort protocolID = reader.ReadUInt16();
                                    ushort messageTag = reader.ReadUInt16();
                                    ushort messageSize = reader.ReadUInt16();
                                    if (messageSize >= maxMessageSize)
                                    {
                                        throw new MessageOverflowException($"A message was received telling it had {messageSize} bytes, which is more than the {maxMessageSize} bytes allowed per message");
                                    }
                                    // But, in order to write the buffer itself, we must wait until we're allowed
                                    // to use it again. The buffer MIGHT still be in use right now, by a former
                                    // command being received.
                                    incomingDataToll.WaitOne();
                                    // Read the whole buffer (i.e. wait for messageSize bytes, and read into a new buffer).
                                    // Also set the incoming metadata variables accordingly.
                                    incomingMessageWriter.ReadAndWrite(reader, messageSize);
                                    // Now we mark, again, as the buffer not being allowed to be used.
                                    // Also, we mark incoming data as present, so the behaviour side can process it.
                                    incomingDataToll.Reset();
                                    isIncomingMessagePresent = true;
                                    inactive = false;
                                }
                                if (stream.CanWrite && trainSending)
                                {
                                    new Writer(stream).ReadAndWrite(new Reader(trainBuffer), trainBuffer.Length);
                                    trainSending = false;
                                    // We must also FORCE the current boarding time to 0 after sucking all of the
                                    // train buffer and set trainSennding to false.
                                    currentBoardingTime = 0;
                                    inactive = false;
                                }
                                if (inactive)
                                {
                                    // On inactivity we sleep a while, to not hog
                                    // the processor.
                                    Thread.Sleep((int)(idleSleepTime * 1000));
                                }
                            }
                        }
                        catch(InvalidOperationException)
                        {
                            // Simply return, for the socket is closed.
                            // This happened, probably, gracefully. The
                            // `finally` block will still do the cleanup.
                            throw new GracefulShutdown();
                        }
                    }
                }
                catch(Exception e)
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
                    if (clientSocket != null)
                    {
                        if (clientSocket.Connected) clientSocket.Close();
                        clientSocket.Dispose();
                        clientSocket = null;
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
                    isIncomingMessagePresent = false;
                    incomingMessageWriter = null;
                    incomingMessageReader = null;
                    incomingMessageBuffer.Dispose();
                    incomingMessageBuffer = null;
                }
            }
        }
    }
}
