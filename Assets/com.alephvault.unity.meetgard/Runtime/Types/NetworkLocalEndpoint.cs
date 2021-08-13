using AlephVault.Unity.Binary;
using AlephVault.Unity.Support.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace AlephVault.Unity.Meetgard
{
    namespace Types
    {
        /// <summary>
        ///   <para>
        ///     A network endpoint serves for local, host,
        ///     connections.
        ///   </para>
        /// </summary>
        public class NetworkLocalEndpoint : NetworkEndpoint
        {
            private bool disposed = false;
            private bool messageSending = false;
            private Action onConnectionStart;
            private Action<ushort, ushort, Reader> onMessage;
            private Action onConnectionEnd;

            /// <summary>
            ///   Tells whether the local endpoint is active.
            ///   Actually, this is the same as checking
            ///   whether the connection is not disposed.
            /// </summary>
            public override bool IsActive => !disposed;

            /// <summary>
            ///   Tells whether the local endpoint is connected.
            ///   Actually, this is the same as checking
            ///   whether the connection is not disposed.
            /// </summary>
            public override bool IsConnected => !disposed;

            /// <summary>
            ///   Closes the local endpoint.
            /// </summary>
            public override void Close()
            {
                disposed = true;
                TriggerOnConnectionEnd();
            }

            /// <summary>
            ///   Sends a stream locally (not by network). This function is asynchronous
            ///   and will wait until no other messages are pending to be sent.
            /// </summary>
            /// <param name="protocolId">The id of protocol for this message</param>
            /// <param name="messageTag">The tag of the message being sent</param>
            /// <param name="content">The input array, typically with a non-zero capacity</param>
            /// <param name="length">The actual length of the content in the array</param>
            public override async Task Send(ushort protocolId, ushort messageTag, byte[] content, int length)
            {
                if (!IsConnected)
                {
                    throw new InvalidOperationException("The endpoint is disposed - No data can be sent");
                }

                if (length > content.Length)
                {
                    throw new ArgumentException($"The actual length of the content ({length}) cannot be greater than the content capacity");
                }


                while (messageSending) await Tasks.Blink();
                messageSending = true;
                Binary.Buffer buffer;

                TriggerOnMessageEvent(protocolId, messageTag, content, length);
            }

            public NetworkLocalEndpoint(Action onConnected, Action<ushort, ushort, Reader> onArrival, Action onDisconnected)
            {
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
                TriggerOnConnectionStart();
            }

            // Asynchronously invokes the onConnectionStart event.
            private async void TriggerOnConnectionStart()
            {
                onConnectionStart?.Invoke();
            }

            // Asynchronously invokes the onConnectionEnd event.
            private async void TriggerOnConnectionEnd()
            {
                onConnectionEnd?.Invoke();
            }

            // Asynchronously invokes the method DoTriggerOnMessageEvent,
            // and then it clears the message buffer.
            private async void TriggerOnMessageEvent(ushort protocolId, ushort messageTag, byte[] content, int length)
            {
                var bufferAndReader = BinaryUtils.ReaderFor(content);
                try
                {
                    // Filling the messageContentUnderlyingBuffer from the network input data.
                    // An exception will be thrown here if fetching the result involves an
                    // attack on message size.
                    Debug.Log($"Processing an incoming message ({protocolId}.{messageTag})");
                    // Now, the message is to be processed.
                    onMessage?.Invoke(protocolId, messageTag, bufferAndReader.Item2);
                }
                finally
                {
                    messageSending = true;
                    // Releasing the buffer, if any. But also giving a warning.
                    if (content != null && length > 0)
                    {
                        Debug.LogWarning($"After processing a NetworkEndpoint incoming message, {length - bufferAndReader.Item1.Position} remained, and were discarded - unexhausted incoming buffers might be a sign of user implementation issues");
                        new Writer(Stream.Null).ReadAndWrite(bufferAndReader.Item2, length - bufferAndReader.Item1.Position);
                    }
                }
            }
        }
    }
}
