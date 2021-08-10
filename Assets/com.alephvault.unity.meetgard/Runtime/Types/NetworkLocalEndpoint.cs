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

            public override async Task Send(ushort protocolId, ushort messageTag, Stream input)
            {
                if (!IsConnected)
                {
                    throw new InvalidOperationException("The endpoint is disposed - No data can be sent");
                }

                while (messageSending) await Tasks.Blink();
                messageSending = true;
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
                TriggerOnMessageEvent(protocolId, messageTag, new Reader(input), buffer);
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
            private async void TriggerOnMessageEvent(ushort protocolId, ushort messageTag, Reader messageContent, Binary.Buffer buffer)
            {
                try
                {
                    onMessage?.Invoke(protocolId, messageTag, messageContent);
                }
                finally
                {
                    messageSending = false;
                    if (buffer.Length > 0)
                    {
                        Debug.LogWarning($"After processing a NetworkEndpoint incoming message, {buffer.Length} remained, and were discarded - unexhausted incoming buffers might be a sign of user implementation issues");
                        new Writer(Stream.Null).ReadAndWrite(messageContent, buffer.Length);
                    }
                }
            }
        }
    }
}
