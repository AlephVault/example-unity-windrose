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
        public partial class NetworkServer : MonoBehaviour
        {
            /// <summary>
            ///   <para>
            ///     This event is triggered after a server successfully
            ///     started (right after successfully start listening
            ///     and accepting incoming connections).
            ///   </para>
            ///   <para>
            ///     This event is triggered in an asynchronous context.
            ///     Any game load should be done before starting it so
            ///     race conditions between new connections and game
            ///     load state do not occur. However, if there are
            ///     no issues regarding that, the game load could
            ///     occur also here.
            ///   </para>
            /// </summary>
            public event Action OnServerStarted = null;

            /// <summary>
            ///   <para>
            ///     This event is triggered after an incoming connection
            ///     was accepted, and registered (and an ID was given).
            ///     This detects both remote and local (host) connections
            ///     (in such cases, it will pass <see cref="HostEndpointId"/>
            ///     as argument).
            ///   </para>
            ///   <para>
            ///     Please note: for id <see cref="HostEndpointId"/>, the
            ///     involved client is the local / host one.
            ///   </para>
            ///   <para>
            ///     This event is triggered in an asynchronous context.
            ///   </para>
            /// </summary>
            public event Action<ulong> OnClientConnected = null;

            /// <summary>
            ///   <para>
            ///     This event is triggered after a client message arrives.
            ///     The arguments for this message are: client id, protocol id,
            ///     message tag, and a buffer reader with the contents.
            ///   </para>
            ///   <para>
            ///     PLEASE NOTE: ONLY ONE HANDLER SHOULD HANDLE THE INCOMING MESSAGE, AND IT
            ///     SHOULD EXHAUST THE BUFFER COMPLETELY.
            ///   </para>
            ///   <para>
            ///     Please note: for id <see cref="HostEndpointId"/>, the
            ///     involved client is the local / host one.
            ///   </para>
            ///   <para>
            ///     This event is triggered in an asynchronous context.
            ///   </para>
            /// </summary>
            public event Action<ulong, ushort, ushort, Reader> OnMessage = null;

            /// <summary>
            ///   <para>
            ///     This event is triggered after a client is disconnected.
            ///     Such client can be a remote endpoint or the local (host)
            ///     endpoint (in such cases, it will pass <see cref="HostEndpointId"/>
            ///     as argument). A non-null exception means that the closure
            ///     was not graceful, but due to an internal error in the
            ///     endpoint connection lifecycle (only meaningful for
            ///     remote endpoints, not the local/host one).
            ///   </para>
            ///   <para>
            ///     Please note: for id <see cref="HostEndpointId"/>, the
            ///     involved client is the local / host one.
            ///   </para>
            ///   <para>
            ///     This event is triggered in an asynchronous context.
            ///   </para>
            /// </summary>
            public event Action<ulong, System.Exception> OnClientDisconnected = null;

            /// <summary>
            ///   <para>
            ///     This event is triggered when a server was told to stop.
            ///     This typically occurs as an error while accepting new
            ///     connections (other errors are in a per-connection basis)
            ///     or when the server Was told to close (in this case, the
            ///     exception will be null). All of the existing endpoints
            ///     were told to close (this does not mean that the respective
            ///     disconnection events were processed for them) before
            ///     this event is triggered. There is nothing to veto here,
            ///     specially in per-connection basis, but just doing a
            ///     global cleanup of the whole server.
            ///   </para>
            ///   <para>
            ///     This event is triggered in an asynchronous context.
            ///   </para>
            /// </summary>
            public event Action<System.Exception> OnServerStopped = null;

            // Asynchronously triggers the OnServerStarted event.
            private async void TriggerOnServerStarted()
            {
                OnServerStarted?.Invoke();
            }

            // Triggers the OnClientConnected event. This occurs in an asynchronous
            // context, already.
            private void TriggerOnClientConnected(ulong clientId)
            {
                OnClientConnected?.Invoke(clientId);
            }

            // Triggers the OnMessage event. This occurs in an asynchronous context,
            // already.
            private void TriggerOnMessage(ulong clientId, ushort protocolId, ushort messageTag, Reader content)
            {
                OnMessage?.Invoke(clientId, protocolId, messageTag, content);
            }

            // Triggers the OnClientDisconnected event. This occurs in an asynchronous
            // context, already, and after the client disconnected and was removed from
            // the internal endpoints registry.
            private void TriggerOnClientDisconnected(ulong clientId, System.Exception reason)
            {
                OnClientDisconnected?.Invoke(clientId, reason);
            }

            // Asynchronously triggers the OnServerStopped event, but after telling
            // all of the active sockets to close. The server stopped event may encounter
            // race conditions with the disconnection events (which become, in turn, calls
            // to TriggerOnClientDisconnected).
            private async void TriggerOnServerStopped(System.Exception e)
            {
                ulong[] keys = endpointById.Keys.ToArray();
                foreach(ulong key in keys)
                {
                    if (endpointById.TryGetValue(key, out NetworkEndpoint value)) value.Close();
                }
                OnServerStopped?.Invoke(e);
            }
        }
    }
}
