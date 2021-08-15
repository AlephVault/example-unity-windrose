using AlephVault.Unity.Binary;
using AlephVault.Unity.Support.Utils;
using System;
using System.Collections;
using System.Collections.Concurrent;
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
            // Whether this fake socket is disposed or not.
            private bool disposed = false;

            // Triggered when this fake socket is just created.
            private Action onConnectionStart;

            // Triggered when this fake socket receives a message.
            private Action<ushort, ushort, ISerializable> onMessage;

            // Triggered when this fake socket is closed.
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

            // The list of queued outgoing messages.
            private ConcurrentQueue<Tuple<ushort, ushort, ISerializable>> queuedOutgoingMessages = new ConcurrentQueue<Tuple<ushort, ushort, ISerializable>>();

            /// <summary>
            ///   Closes the local endpoint.
            /// </summary>
            public override void Close()
            {
                disposed = true;
                TriggerOnConnectionEnd();
            }

            /// <summary>
            ///   Queues the message to be sent. It only asks for a mutex
            /// </summary>
            /// <param name="protocolId">The id of protocol for this message</param>
            /// <param name="messageTag">The tag of the message being sent</param>
            /// <param name="data">The object to serialize and send</param>
            protected override async Task DoSend(ushort protocolId, ushort messageTag, ISerializable data)
            {
                queuedOutgoingMessages.Enqueue(new Tuple<ushort, ushort, ISerializable>(protocolId, messageTag, data));
                TriggerOnMessageEvent();
            }

            public NetworkLocalEndpoint(Action onConnected, Action<ushort, ushort, ISerializable> onArrival, Action onDisconnected)
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

            // Asynchronously pops a message from the list and
            // triggers the event. In this case, the order will
            // be guaranteed.
            private async void TriggerOnMessageEvent()
            {
                if (queuedOutgoingMessages.TryDequeue(out var result))
                {
                    onMessage?.Invoke(result.Item1, result.Item2, result.Item3);
                }
            }
        }
    }
}
