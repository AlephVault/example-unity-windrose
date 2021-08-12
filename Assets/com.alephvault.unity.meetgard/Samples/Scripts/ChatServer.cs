using AlephVault.Unity.Binary;
using AlephVault.Unity.Meetgard.Server;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlephVault.Unity.Meetgard
{
    namespace Samples
    {
        /// <summary>
        ///   <para>
        ///     This is just a sample chat server, with no channels.
        ///     It receives client connections (and perhaps a host
        ///     connection), globally echoes messages from those
        ///     clients, and globally broadcasts some ping messages
        ///     which are replied by pong messages by the clients.
        ///   </para>
        /// </summary>
        [RequireComponent(typeof(NetworkServer))]
        public class ChatServer : MonoBehaviour
        {
            private NetworkServer server;
            private Buffer buffer = new Buffer(1024);
            private HashSet<ulong> failedEndpoints = new HashSet<ulong>();
            private Coroutine pingPong;

            [SerializeField]
            private KeyCode startKey;

            [SerializeField]
            private KeyCode stopKey;

            private void Awake()
            {
                server = GetComponent<NetworkServer>();
            }

            private void Start()
            {
                server.OnServerStarted += Server_OnServerStarted;
                server.OnClientConnected += Server_OnClientConnected;
                server.OnMessage += Server_OnMessage;
                server.OnClientDisconnected += Server_OnClientDisconnected;
                server.OnServerStopped += Server_OnServerStopped;
            }

            private void Update()
            {
                if (Input.GetKeyDown(startKey) && !server.IsListening) server.StartServer(6666);
                if (Input.GetKeyDown(stopKey) && server.IsListening) server.StopServer();
            }

            private void Server_OnServerStarted()
            {
                Debug.Log("Server :: Just Started");
                pingPong = StartCoroutine(PingPong());
            }

            private IEnumerator PingPong()
            {
                while(true)
                {
                    yield return new WaitForSeconds(1f);
                    Echo echo = new Echo();
                    echo.Content = "PING";
                    echo.Serialize(new Serializer(new Writer(buffer)));
                    server.TryBroadcast(null, 0, 1, buffer, failedEndpoints);
                }
            }

            private void Server_OnClientConnected(ulong clientId)
            {
                Debug.Log($"Server :: Just Connected: {clientId}");
            }

            private void Server_OnMessage(ulong arg1, ushort arg2, ushort arg3, Reader arg4)
            {
                Debug.Log($"Server :: Data arrival from {arg1} is {arg2}.{arg3}");
                if (arg3 == 0)
                {
                    // A message.
                    Message message = new Message();
                    message.Serialize(new Serializer(arg4));
                    Debug.Log("Server :: Received message: " + message.Content);
                    buffer.Seek(0, System.IO.SeekOrigin.Begin);
                    message.Serialize(new Serializer(new Writer(buffer)));
                    server.TryBroadcast(null, 0, 0, buffer, failedEndpoints);
                }
                else if (arg3 == 1)
                {
                    // A pong.
                    Echo echo = new Echo();
                    echo.Serialize(new Serializer(arg4));
                    Debug.Log("Server :: Received pong: " + echo.Content);
                }
            }

            private void Server_OnClientDisconnected(ulong clientId, System.Exception cause)
            {
                Debug.Log($"Server :: Just Disconnected: {clientId} with cause: {cause}");
            }

            private void Server_OnServerStopped(System.Exception cause)
            {
                Debug.Log($"Server :: Just Stopped with cause: {cause}");
                if (pingPong != null)
                {
                    StopCoroutine(pingPong);
                    pingPong = null;
                }
            }
        }
    }
}
