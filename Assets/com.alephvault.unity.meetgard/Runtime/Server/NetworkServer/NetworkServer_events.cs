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

            // Asynchronously triggers the OnServerStarted event.
            private async void DoTriggerOnServerStarted()
            {
                TriggerOnServerStarted();
            }

            // Triggers the OnMessage event. This occurs in an asynchronous context,
            // already.
            private void TriggerOnMessage(ulong clientId, ushort protocolId, ushort messageTag, Reader content)
            {
                OnMessage?.Invoke(clientId, protocolId, messageTag, content);
            }

            // Asynchronously triggers the OnServerStopped event, but after telling
            // all of the active sockets to close. The server stopped event may encounter
            // race conditions with the disconnection events (which become, in turn, calls
            // to TriggerOnClientDisconnected).
            private async void DoTriggerOnServerStopped(System.Exception e)
            {
                CloseAllEndpoints();
                TriggerOnServerStopped(e);
            }
        }
    }
}
