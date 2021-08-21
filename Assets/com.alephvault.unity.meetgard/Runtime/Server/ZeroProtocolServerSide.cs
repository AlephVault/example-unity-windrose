
using AlephVault.Unity.Meetgard.Protocols;
using AlephVault.Unity.Meetgard.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AlephVault.Unity.Meetgard
{
    namespace Server
    {
        /// <summary>
        ///   Server-side implementation for the "zero" protocol.
        /// </summary>
        public class ZeroProtocolServerSide : ProtocolServerSide<ZeroProtocolDefinition>
        {
            /// <summary>
            ///   A value telling the version of the current protocol
            ///   set in this network server. This must be changed as
            ///   per deployment, since certain game changes are meant
            ///   to be not retro-compatible and thus the version must
            ///   be marked as mismatching.
            /// </summary>
            [SerializeField]
            private Protocols.Version Version;

            /// <summary>
            ///   The timeout to wait for a MyVersion message.
            /// </summary>
            [SerializeField]
            private float timeout = 3f;

            // Tells the connections that are ready to interact (i.e.
            // have their version handshake completed and approved).
            private HashSet<ulong> readyConnections = new HashSet<ulong>();

            private Func<ulong, Nothing, Task> SendLetsAgree;
            private Func<ulong, Nothing, Task> SendTimeout;
            private Func<ulong, Nothing, Task> SendVersionMatch;
            private Func<ulong, Nothing, Task> SendVersionMismatch;
            private Func<ulong, Nothing, Task> SendNotReady;
            private Func<ulong, Nothing, Task> SendAlreadyDone;

            protected new void Awake()
            {
                base.Awake();
                SendLetsAgree = MakeSender<Nothing>("LetsAgree");
                SendTimeout = MakeSender<Nothing>("Timeout");
                SendVersionMatch = MakeSender<Nothing>("VersionMatch");
                SendVersionMismatch = MakeSender<Nothing>("VersionMismatch");
                SendNotReady = MakeSender<Nothing>("NotReady");
                SendAlreadyDone = MakeSender<Nothing>("AlreadyDone");
            }

            /// <summary>
            ///   Tells whether a particular client id is "ready" or
            ///   not (i.e. still connected and in the set of ready
            ///   connections: with its version handshake approved).
            /// </summary>
            /// <param name="clientId">The client id to check</param>
            /// <returns>Whether it is ready or not</returns>
            public bool Ready(ulong clientId)
            {
                return readyConnections.Contains(clientId);
            }

            public override async Task OnConnected(ulong clientId)
            {
                readyConnections.Remove(clientId);
                await SendLetsAgree(clientId, new Nothing());
                StartTimeout(clientId);
            }

            // This is intentionally intended to be a separate task.
            private async void StartTimeout(ulong clientId)
            {
                await Task.Delay((int)(1000 * timeout));
                if (!Ready(clientId))
                {
                    await SendTimeout(clientId, new Nothing());
                    server.Close(clientId);
                }
            }

            public override async Task OnDisconnected(ulong clientId, System.Exception reason)
            {
                readyConnections.Remove(clientId);
            }

            protected override void SetIncomingMessageHandlers()
            {
                AddIncomingMessageHandler<Protocols.Version>("MyVersion", async (proto, clientId, version) =>
                {
                    if (Ready(clientId))
                    {
                        await SendAlreadyDone(clientId, new Nothing());
                    }
                    else if (version.Equals(Version))
                    {
                        await SendVersionMatch (clientId, new Nothing());
                        readyConnections.Add(clientId);
                    }
                    else
                    {
                        await SendVersionMismatch (clientId, new Nothing());
                        server.Close(clientId);
                    }
                });
            }
        }
    }
}