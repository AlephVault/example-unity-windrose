using AlephVault.Unity.Binary;
using AlephVault.Unity.Meetgard.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlephVault.Unity.Meetgard
{
    namespace Samples
    {
        /// <summary>
        ///   This is just the client for that chat. It receives
        ///   pings and message broadcasts from the server, and
        ///   logs them. It also replies pings with pongs, and
        ///   sends new messages to the server.
        /// </summary>
        [RequireComponent(typeof(NetworkClient))]
        public class ChatClient : MonoBehaviour
        {
            private NetworkClient client;

            [SerializeField]
            private string name;

            [SerializeField]
            private KeyCode connectKey;

            [SerializeField]
            private KeyCode disconnectKey;

            [SerializeField]
            private KeyCode helloKey;

            private byte[] buffer = new byte[1024];

            private void Awake()
            {
                client = GetComponent<NetworkClient>();
            }

            private void Start()
            {
                client.OnConnected += Client_OnConnected;
                client.OnMessage += Client_OnMessage;
                client.OnDisconnected += Client_OnDisconnected;
            }

            private void Update()
            {
                if (Input.GetKeyDown(connectKey) && !client.IsConnected) client.Connect("localhost", 6666);
                if (Input.GetKeyDown(helloKey) && client.IsConnected)
                {
                    Debug.Log($"Client({name}) :: Sending message");
                    Message message = new Message();
                    message.Content = $"Hello, I'm {name}";
                    var bufferAndWriter = BinaryUtils.WriterFor(buffer);
                    message.Serialize(new Serializer(bufferAndWriter.Item2));
                    client.Send(0, 0, buffer, (int)bufferAndWriter.Item1.Position);
                }
                if (Input.GetKeyDown(disconnectKey) && client.IsConnected) client.Close();
            }

            private void Client_OnConnected()
            {
                Debug.Log($"Client({name}) :: Successfully connected to server");
            }

            private void Client_OnMessage(ushort arg1, ushort arg2, Reader arg3)
            {
                Debug.Log($"Client({name}) :: Data arrival {arg1}.{arg2}");
                if (arg2 == 0)
                {
                    // A message.
                    Message message = new Message();
                    message.Serialize(new Serializer(arg3));
                    Debug.Log($"Client({name}) :: Received message: {message.Content}");
                }
                else if (arg2 == 1)
                {
                    // A ping.
                    Echo echo = new Echo();
                    echo.Serialize(new Serializer(arg3));
                    Debug.Log($"Client({name}) :: Received pong: " + echo.Content);
                    echo.Content = $"PONG {name}";
                    long size = BinaryUtils.Dump(echo, buffer);
                    client.Send(0, 1, buffer, (short)size);
                }
            }

            private void Client_OnDisconnected(System.Exception cause)
            {
                Debug.Log($"Client({name}) :: Successfully disconnected from server, with cause: {cause}");
            }
        }
    }
}
