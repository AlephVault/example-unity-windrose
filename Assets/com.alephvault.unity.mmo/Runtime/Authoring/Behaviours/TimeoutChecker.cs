using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Connection;
using AlephVault.Unity.Support.Utils;
using MLAPI.Messaging;
using System.IO;
using MLAPI.Serialization;
using MLAPI.Serialization.Pooled;

namespace AlephVault.Unity.MMO
{
    namespace Authoring
    {
        namespace Behaviours
        {
            [RequireComponent(typeof(NetworkManager))]
            public partial class TimeoutChecker : MonoBehaviour
            {
                private const string Ping = "PING";
                private const string Pong = "PONG";

                private NetworkManager manager;

                [SerializeField]
                private float pingPongInterval = 15.0f;

                [SerializeField]
                private uint tolerance = 2;

                private float clientTolerance;

                private Dictionary<ulong, uint> connectedClientsPendingPings = new Dictionary<ulong, uint>();

                private float currentServerLoopTime = 0f;
                private float currentClientLostTime = 0f;

                private void Awake()
                {
                    clientTolerance = tolerance * pingPongInterval;
                    manager = GetComponent<NetworkManager>();
                }

                private void Start()
                {
                    pingPongInterval = Values.Max(pingPongInterval, 10f);
                    tolerance = Values.Max(tolerance, 2u);
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
                            CustomMessagingManager.SendNamedMessage(Pong, manager.ServerClientId, buffer);
                        }
                        currentClientLostTime = 0;
                    }
                }

                private void OnClientConnected(ulong clientId)
                {
                    if (manager.IsServer)
                    {
                        // We initialize a new entry in server mode
                        // for the newly connected client to handle
                        // its timeout.
                        connectedClientsPendingPings[clientId] = 0;
                    }
                    else if (manager.IsClient)
                    {
                        // We always need to reset to 0 the client
                        // counter in this case, so it starts the
                        // update (in client mode) appropriately.
                        currentClientLostTime = 0;
                    }
                }

                private void OnClientDisconnect(ulong clientId)
                {
                    if (manager.IsServer)
                    {
                        // Since the client was removed, we must remove
                        // the client entry from the server.
                        connectedClientsPendingPings.Remove(clientId);
                    }
                    else if (manager.IsClient)
                    {
                        // We, again, set our lost timeouts to 0.
                        currentClientLostTime = 0;
                    }
                }

                private void Update()
                {
                    if (manager.IsServer)
                    {
                        currentServerLoopTime += Time.unscaledDeltaTime;
                        if (currentServerLoopTime > pingPongInterval)
                        {
                            currentServerLoopTime -= pingPongInterval;
                            foreach (KeyValuePair<ulong, NetworkClient> pair in manager.ConnectedClients)
                            {
                                if (pair.Key != manager.LocalClientId)
                                {
                                    // In server mode, if there were {t} elapsed timeouts
                                    // without receiving a pong message, then we close the
                                    // client connection.
                                    if (!connectedClientsPendingPings.ContainsKey(pair.Key)) connectedClientsPendingPings[pair.Key] = 0;
                                    if (connectedClientsPendingPings[pair.Key] >= tolerance)
                                    {
                                        manager.DisconnectClient(pair.Key);
                                        connectedClientsPendingPings[pair.Key] = 0;
                                    }
                                    else
                                    {
                                        connectedClientsPendingPings[pair.Key] += 1;
                                        using (NetworkBuffer buffer = PooledNetworkBuffer.Get())
                                        {
                                            CustomMessagingManager.SendNamedMessage(Ping, pair.Key, buffer);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (manager.IsClient)
                    {
                        // In client mode, if there were {t}*I elapsed seconds
                        // without receiving a single ping message, then we
                        // close the connection.
                        currentClientLostTime += Time.unscaledDeltaTime;
                        if (currentClientLostTime >= clientTolerance)
                        {
                            manager.StopClient();
                        }
                    }
                }
            }
        }
    }
}