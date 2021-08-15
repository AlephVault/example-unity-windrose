using System;

namespace AlephVault.Unity.Meetgard
{
    namespace Types
    {
        using AlephVault.Unity.Binary;

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
        public partial class NetworkRemoteEndpoint : NetworkEndpoint
        {
            // When a connection is established, this callback is processed.
            private Action onConnectionStart = null;

            // When a message is received, this callback is processed, passing
            // a protocol ID, a message tag, and a reader for the incoming buffer.
            private Action<ushort, ushort, ISerializable> onMessage = null;

            // When a connection is terminated, this callback is processed.
            // If the termination was not graceful, the exception that caused
            // the termination will be given. Otherwise, it will be null.
            private Action<System.Exception> onConnectionEnd = null;

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
        }
    }
}
