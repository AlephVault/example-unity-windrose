using AlephVault.Unity.Binary;
using AlephVault.Unity.Meetgard.Server;
using AlephVault.Unity.Meetgard.Types;
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
            private class SampleMessage : ISerializable
            {
                public int IntegerValue = 1000000;
                public string StringValue = "Sample String";
                public bool BoolValue = false;
                public float FloatValue = 4.0f;

                public void Serialize(Serializer serializer)
                {
                    serializer.Serialize(ref IntegerValue);
                    serializer.Serialize(ref StringValue);
                    serializer.Serialize(ref BoolValue);
                    serializer.Serialize(ref FloatValue);
                }

                public override bool Equals(object obj)
                {
                    return (obj is SampleMessage obj2) && (obj2.BoolValue == BoolValue)
                        && (obj2.FloatValue == FloatValue) && (obj2.IntegerValue == IntegerValue)
                        && (obj2.StringValue == StringValue);
                }
            }

            private NetworkServer server;
            private byte[] buffer = new byte[1024];
            private HashSet<ulong> failedEndpoints = new HashSet<ulong>();
            private Coroutine pingPong;

            [SerializeField]
            private KeyCode startKey;

            [SerializeField]
            private KeyCode stopKey;

            [SerializeField]
            private KeyCode handshakeTestKey;

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
                if (Input.GetKeyDown(handshakeTestKey)) TestHandshake();
            }

            private void TestHandshake()
            {
                SampleMessage sentMessage = new SampleMessage() { BoolValue = true, FloatValue = 1.0f, StringValue = "Hello", IntegerValue = 7 };
                byte[] tmpArray = new byte[1024];
                Buffer sampleStream = new Buffer(6144);
                MessageUtils.WriteMessage(sampleStream, 3, 4, sentMessage, tmpArray);

                // Clear streams and fill array.
                sampleStream.Seek(0, System.IO.SeekOrigin.Begin);
                for (int i = 0; i < 1024; i++) tmpArray[i] = 0;

                // Get the message.
                var result = MessageUtils.ReadMessage(sampleStream, (pid, mt) => new SampleMessage(), tmpArray);
                SampleMessage receivedMessage = (SampleMessage)result.Item2;
                Debug.Log($"Protocol id: {result.Item1.ProtocolId} vs 3, Message tag: {result.Item1.MessageTag} vs 4, Object equality: {receivedMessage.Equals(sentMessage)}");
                Debug.Log($"Object equality details: Integer: {sentMessage.IntegerValue} vs {receivedMessage.IntegerValue}, Float: {sentMessage.FloatValue} vs {receivedMessage.FloatValue}, " +
                          $"String: '{sentMessage.StringValue}' vs '{receivedMessage.StringValue}', Bool: {sentMessage.BoolValue} vs {receivedMessage.BoolValue}");
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
                    long length = BinaryUtils.Dump(echo, buffer);
                    server.TryBroadcast(null, 0, 1, buffer, (ushort)length, failedEndpoints);
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
                    Debug.Log("Server :: Received message");
                    // A message.
                    Message message = new Message();
                    try
                    {
                        message.Serialize(new Serializer(arg4));
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError(e);
                    }
                    Debug.Log("Server :: Message content is: " + message.Content);
                    long length = BinaryUtils.Dump(message, buffer);
                    server.TryBroadcast(null, 0, 0, buffer, (ushort)length, failedEndpoints);
                }
                else if (arg3 == 1)
                {
                    Debug.Log("Server :: Received pong");
                    // A pong.
                    Echo echo = new Echo();
                    echo.Serialize(new Serializer(arg4));
                    Debug.Log("Server :: Pong is: " + echo.Content);
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
