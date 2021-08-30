using AlephVault.Unity.Meetgard.Client;
using UnityEngine;

namespace AlephVault.Unity.Meetgard.Auth
{
    namespace Samples
    {
        /// <summary>
        ///   This behaviour lets manage the client side via
        ///   some keys in the inspector.
        /// </summary>
        [RequireComponent(typeof(SampleAuthChatProtocolClientSide))]
        public class SampleAuthChatClientKeys : MonoBehaviour
        {
            [SerializeField]
            private KeyCode connectKey;

            [SerializeField]
            private KeyCode disconnectKey;

            [SerializeField]
            private KeyCode sendMessageKey;

            private SampleAuthChatProtocolClientSide protocol;
            private NetworkClient client;

            // Start is called before the first frame update
            void Awake()
            {
                protocol = GetComponent<SampleAuthChatProtocolClientSide>();
                client = GetComponent<NetworkClient>();
            }

            // Update is called once per frame
            void Update()
            {
                if (Input.GetKeyDown(connectKey) && !client.IsConnected)
                {
                    client.Connect("127.0.0.1", 6666);
                }
                else if (Input.GetKeyDown(disconnectKey) && client.IsConnected)
                {
                    client.Close();
                }
                else if (Input.GetKeyDown(sendMessageKey) && client.IsConnected)
                {
                    protocol.Say("Lorem ipsum dolor sit amet");
                }
            }
        }
    }
}
