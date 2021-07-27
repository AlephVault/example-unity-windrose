using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Connection;
using MLAPI.Messaging;
using MLAPI.Serialization;
using MLAPI.Serialization.Pooled;
using AlephVault.Unity.Support.Utils;

namespace AlephVault.Unity.MMO
{
    namespace Authoring
    {
        namespace Behaviours
        {
            /// <summary>
            ///   <para>
            ///     A timeout checker works on both sides of the
            ///     connection: both in server/host and in client.
            ///     In both sides, checks at a specific given time
            ///     interval will be done, and a ping/pong handshake
            ///     will be attempted. If the server or the client
            ///     fail to complete a certain number of handshakes,
            ///     the connection will be terminated on either side.
            ///   </para>
            ///   <para>
            ///     Another check is to be done on client side:
            ///     a timeout setting for when a client attempts
            ///     a connection to a server (i.e. is client, but
            ///     is not server), but still does not fulfill the
            ///     connection (i.e. a connection timeout).
            ///   </para>
            /// </summary>
            [RequireComponent(typeof(NetworkManager))]
            public partial class TimeoutChecker : MonoBehaviour
            {
                private const string Ping = "__AV:MMO__:PING";
                private const string Pong = "__AV:MMO__:PONG";

                private NetworkManager manager;

                /// <summary>
                ///   The interval, in seconds, between each
                ///   ping/pong handshake.
                /// </summary>
                [SerializeField]
                private float pingPongInterval = 15.0f;

                /// <summary>
                ///   The tolerance of "lost" pings it tolerates
                ///   in a per-client basis. For clients, it is
                ///   multiplied by the time interval to get the
                ///   total time the client will resist without
                ///   receiving any "ping" handshake from the
                ///   server before disconnection.
                /// </summary>
                [SerializeField]
                private uint pingTolerance = 2;

                // Contains the whole maximum time a client can
                // tolerate not receiving any ping command from
                // the server it is connected to.
                private float finalClientPingTolerance;

                /// <summary>
                ///   Triggered when the client is connected but
                ///   it did not receive a ping command from the
                ///   server after the full tolerance time.
                /// </summary>
                public event Action OnConnectedClientTimeout = null;

                private Dictionary<ulong, uint> connectedClientsPendingPings = new Dictionary<ulong, uint>();

                // Tracks the current interval time since the last
                // ping command sent to all the clients.
                private float currentServerPingLoopTime = 0f;

                // Tracks the current time since the last ping
                // command received from the server. This time
                // is compared against finalClientPingTolerance.
                private float currentClientPingLostTime = 0f;

                // There is something else to implement: A timeout
                // for when the client attempts a connection to
                // the server, but the server is not ready or
                // fails to respond appropriately.

                /// <summary>
                ///   The tolerance (in seconds) for a client
                ///   connection, since it is started against
                ///   a server, to not disconnect while the
                ///   server is not ready or attending the
                ///   connection. After this time, if the
                ///   server did not attend the connection,
                ///   this client will be stopped.
                /// </summary>
                [SerializeField]
                private float clientConnectionTolerance = 5f;

                // Tracks how much time elapsed since the manager
                // becoming remote client. It will be matched
                // against the corresponding timeout.
                private float currentClientPendingLostTime = 0f;

                /// <summary>
                ///   Triggered when the client is connecting but
                ///   it did not receive a full handle from the
                ///   server after the "pending" tolerance time.
                ///   This is typically caused by the server not
                ///   being ready (i.e. listening).
                /// </summary>
                public event Action OnConnectingClientTimeout = null;

                // Whether the manager is connected as remote
                // client (and NOT as host), successfully (this
                // means: its request was approved by a server).
                private bool successfullyConnectedAsClientOnly = false;

                private void Awake()
                {
                    finalClientPingTolerance = pingTolerance * pingPongInterval;
                    manager = GetComponent<NetworkManager>();
                }

                private void Start()
                {
                    pingPongInterval = Values.Max(pingPongInterval, 10f);
                    pingTolerance = Values.Max(pingTolerance, 2u);
                    clientConnectionTolerance = Values.Max(clientConnectionTolerance, 1f);
                    manager.OnClientConnectedCallback += OnClientConnected;
                    manager.OnClientDisconnectCallback += OnClientDisconnect;
                    CustomMessagingManager.RegisterNamedMessageHandler(Pong, OnPongArrived);
                    CustomMessagingManager.RegisterNamedMessageHandler(Ping, OnPingArrived);
                }

                private void OnDestroy()
                {
                    if (manager)
                    {
                        manager.OnClientConnectedCallback -= OnClientConnected;
                        manager.OnClientDisconnectCallback -= OnClientDisconnect;
                    }
                    CustomMessagingManager.UnregisterNamedMessageHandler(Pong);
                    CustomMessagingManager.UnregisterNamedMessageHandler(Ping);
                }

                private void OnPongArrived(ulong clientId, Stream stream)
                {
                    Debug.Log("PONG arrived");
                    if (manager.IsServer && clientId != manager.LocalClientId)
                    {
                        connectedClientsPendingPings[clientId] = 0;
                    }
                }

                private void OnPingArrived(ulong clientId, Stream stream)
                {
                    if (manager.IsClient && !manager.IsServer)
                    {
                        using (NetworkBuffer buffer = PooledNetworkBuffer.Get())
                        {
                            Debug.Log("PONG sending");
                            CustomMessagingManager.SendNamedMessage(Pong, manager.ServerClientId, buffer);
                        }
                        currentClientPingLostTime = 0;
                    }
                }

                private void OnClientConnected(ulong clientId)
                {
                    if (manager.IsServer)
                    {
                        Debug.LogFormat("Client {0} connected", clientId);
                        // We initialize a new entry in server mode
                        // for the newly connected client to handle
                        // its timeout.
                        connectedClientsPendingPings[clientId] = 0;
                    }
                    else if (manager.IsClient)
                    {
                        Debug.Log("Connected to server");
                        // We always need to reset to 0 the client
                        // counter in this case, so it starts the
                        // update (in client mode) appropriately.
                        currentClientPingLostTime = 0;
                        successfullyConnectedAsClientOnly = true;
                    }
                }

                private void OnClientDisconnect(ulong clientId)
                {
                    if (manager.IsServer)
                    {
                        // Since the client was removed, we must remove
                        // the client entry from the server.
                        Debug.LogFormat("Client {0} disconnected", clientId);
                        connectedClientsPendingPings.Remove(clientId);
                    }
                    else if (manager.IsClient)
                    {
                        // We, again, set our lost timeouts to 0.
                        Debug.Log("Disconnected from server");
                        currentClientPingLostTime = 0;
                        successfullyConnectedAsClientOnly = false;
                    }
                }

                private void Update()
                {
                    if (manager.IsServer)
                    {
                        currentServerPingLoopTime += Time.unscaledDeltaTime;
                        if (currentServerPingLoopTime > pingPongInterval)
                        {
                            currentServerPingLoopTime -= pingPongInterval;
                            foreach (KeyValuePair<ulong, NetworkClient> pair in manager.ConnectedClients)
                            {
                                if (pair.Key != manager.LocalClientId)
                                {
                                    Debug.LogFormat("Ping test for client id: {0}", pair.Key);
                                    // In server mode, if there were {t} elapsed timeouts
                                    // without receiving a pong message, then we close the
                                    // client connection.
                                    if (!connectedClientsPendingPings.ContainsKey(pair.Key)) connectedClientsPendingPings[pair.Key] = 0;
                                    if (connectedClientsPendingPings[pair.Key] >= pingTolerance)
                                    {
                                        Debug.LogFormat("Disconnecting client {0}", pair.Key);
                                        manager.DisconnectClient(pair.Key);
                                        connectedClientsPendingPings[pair.Key] = 0;
                                    }
                                    else
                                    {
                                        connectedClientsPendingPings[pair.Key] += 1;
                                        using (NetworkBuffer buffer = PooledNetworkBuffer.Get())
                                        {
                                            Debug.Log("PING sending");
                                            CustomMessagingManager.SendNamedMessage(Ping, pair.Key, buffer);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (manager.IsClient)
                    {
                        if (successfullyConnectedAsClientOnly)
                        {
                            // In client mode, if there were {t}*I elapsed seconds
                            // without receiving a single ping message, then we
                            // close the connection.
                            currentClientPingLostTime += Time.unscaledDeltaTime;
                            if (currentClientPingLostTime >= finalClientPingTolerance)
                            {
                                Debug.Log("Disconnecting from server");
                                OnConnectedClientTimeout?.Invoke();
                                manager.StopClient();
                            }
                            // The pending connection timeout will always be 0 here.
                            currentClientPendingLostTime = 0;
                        }
                        else
                        {
                            // In client mode, but not-yet-successful, we will count
                            // toward an eventual timeout of the connection to the
                            // server (as an attempt). If it goes beyond a timeout,
                            // then we close the client connection.
                            currentClientPendingLostTime += Time.unscaledDeltaTime;
                            if (currentClientPendingLostTime > clientConnectionTolerance)
                            {
                                currentClientPendingLostTime = 0;
                                OnConnectingClientTimeout?.Invoke();
                                manager.StopClient();
                            }
                        }
                    }
                }
            }
        }
    }
}
