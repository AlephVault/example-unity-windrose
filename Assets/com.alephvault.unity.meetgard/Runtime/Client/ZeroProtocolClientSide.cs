
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
    namespace Client
    {
        /// <summary>
        ///   Client-side implementation for the "zero" protocol.
        /// </summary>
        public class ZeroProtocolClientSide : ProtocolClientSide<ZeroProtocolDefinition>
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
            ///   Tells whether this client is ready or not (i.e.
            ///   whether it passed the version check for the current
            ///   connection, or not).
            /// </summary>
            public bool Ready { get; private set; }

            protected override void SetIncomingMessageHandlers()
            {
                AddIncomingMessageHandler<Nothing>("LetsAgree", (proto, _) =>
                {
                    Send("MyVersion", Version);
                    // This will be invoked after the client repied with MyVersion
                    // message. This means: after the handshake started in client
                    // (protocol-wise) side.
                    OnZeroHandshakeStarted?.Invoke();
                });
                AddIncomingMessageHandler<Nothing>("Timeout", (proto, _) =>
                {
                    // This may be invoked regardless the LetsAgree being received
                    // or the MyVersion message being sent. This is due to the
                    // client taking too long to respond to LetsAgree message.
                    // Expect a disconnection after this message.
                    OnTimeout?.Invoke();
                });
                AddIncomingMessageHandler<Nothing>("VersionMatch", (proto, _) =>
                {
                    // The version was matched. Don't worry: we will seldom make
                    // use of this event, since typically other protocols will
                    // in turn initialize on their own for this client and send
                    // their own messages. But it is available anyway.
                    OnVersionMatch?.Invoke();
                });
                AddIncomingMessageHandler<Nothing>("VersionMismatch", (proto, _) =>
                {
                    // This message is received when there is a mismatch between
                    // the server version and the client version. After receiving
                    // this message, expect a sudden graceful disconnection.
                    OnVersionMismatch?.Invoke();
                });
                AddIncomingMessageHandler<Nothing>("NotReady", (proto, _) =>
                {
                    // This is a debug message. Typically, it involves rejecting
                    // any message other than MyVersion, since the protocols are
                    // not ready for this client (being ready occurs after
                    // agreeing with this zero protocol).
                    OnNotReadyError?.Invoke();
                });
            }

            /// <summary>
            ///   Triggered when the client received a LetsAgree message and replied
            ///   with MyVersion message.
            /// </summary>
            public event Action OnZeroHandshakeStarted = null;

            /// <summary>
            ///   Triggered when the client received the notification that the
            ///   version handshake was correct.
            /// </summary>
            public event Action OnVersionMatch = null;

            /// <summary>
            ///   Triggered when the client received the notification that the
            ///   version handshake was incorrect. Expect a sudden yet graceful
            ///   disconnection after this message.
            /// </summary>
            public event Action OnVersionMismatch = null;

            /// <summary>
            ///   Triggered when the client attempted any message other than
            ///   MyVersion message while the handshake is still not successfully
            ///   completed in either side.
            /// </summary>
            public event Action OnNotReadyError = null;

            /// <summary>
            ///   Triggered when the client received the notification that the
            ///   version handshake did not occur after a tolerance time, perhaps
            ///   due to malicius attempts or networking problems. Expect a sudden
            ///   yet graceful disconnection after this message.
            /// </summary>
            public event Action OnTimeout = null;
        }
    }
}