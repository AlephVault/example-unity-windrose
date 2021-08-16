using AlephVault.Unity.Support.Utils;
using System;
using UnityEngine;

namespace AlephVault.Unity.Meetgard
{
    namespace Client
    {
        using AlephVault.Unity.Binary;
        using AlephVault.Unity.Meetgard.Types;
        using System.Net;
        using System.Net.Sockets;
        using System.Threading.Tasks;

        /// <summary>
        ///   <para>
        ///     Network clients are behaviours that spawn an additional
        ///     thread to interact with a server. They can be connected
        ///     to only one server at once, but many clients can be
        ///     instantiated in the same scene.
        ///   </para>
        /// </summary>
        public partial class NetworkClient : MonoBehaviour
        {
            /// <summary>
            ///   <para>
            ///     The time to sleep, on each iteration, when no data to
            ///     read or write is present in the socket on a given
            ///     iteration.
            ///   </para>
            ///   <para>
            ///     This setting should match whatever is set in the server
            ///     and supported by the protocols to use.
            ///   </para>
            /// </summary>
            [SerializeField]
            private float idleSleepTime = 0.01f;

            /// <summary>
            ///   <para>
            ///     The maximum size of each individual message to be sent.
            ///   </para>
            ///   <para>
            ///     This setting should match whatever is set in the server
            ///     and supported by the protocols to use.
            ///   </para>
            /// </summary>
            [SerializeField]
            private ushort maxMessageSize = 1024;

            // The underlying network endpoint, or null if the connection
            // is not established.
            private NetworkRemoteEndpoint endpoint = null;

            /// <summary>
            ///   Tells whether the endpoint is active or not. While Active, another
            ///   life-cycle (e.g. a call to <see cref="Connect(IPAddress, int)"/> or
            ///   <see cref="Connect(string, int)"/>) cannot be done.
            /// </summary>
            public bool IsRunning { get { return endpoint != null && endpoint.IsActive; } }

            /// <summary>
            ///   Tells whether the underlying socket is instantiated and connected.
            /// </summary>
            public bool IsConnected { get { return endpoint != null && endpoint.IsConnected; } }
            
            private void Awake()
            {
                maxMessageSize = Values.Clamp(512, maxMessageSize, 6144);
                idleSleepTime = Values.Clamp(0.005f, idleSleepTime, 0.5f);
                SetupClientProtocols();
            }

            private void OnDestroy()
            {
                if (IsConnected) Close();
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
                if (IsRunning)
                {
                    throw new InvalidOperationException("The socket is already connected - It cannot be connected again");
                }

                // Connects to a given address. Throws any exception
                // that socket connection throws.
                TcpClient client = new TcpClient();
                client.Connect(address, port);
                endpoint = new NetworkRemoteEndpoint(
                    client, NewMessageContainer, TriggerOnConnected, TriggerOnMessage, TriggerOnDisconnected,
                    maxMessageSize, idleSleepTime
                );
            }

            /// <summary>
            ///   Sends a stream through the network. This function is asynchronous
            ///   and will wait until no other messages are pending to be sent.
            /// </summary>
            /// <param name="protocolId">The id of protocol for this message</param>
            /// <param name="messageTag">The tag of the message being sent</param>
            /// <param name="content">The input array, typically with a non-zero capacity</param>
            public Task Send(ushort protocolId, ushort messageTag, ISerializable content)
            {
                if (!IsRunning)
                {
                    throw new InvalidOperationException("The endpoint is not running - No data can be sent");
                }

                return endpoint.Send(protocolId, messageTag, content);
            }

            /// <summary>
            ///   Closes the active connection, if any. This, actually,
            ///   tells the thread to close the connection.
            /// </summary>
            public void Close()
            {
                if (!IsRunning)
                {
                    throw new InvalidOperationException("The socket is not connected - It cannot be closed");
                }

                endpoint.Close();
            }
        }
    }
}
