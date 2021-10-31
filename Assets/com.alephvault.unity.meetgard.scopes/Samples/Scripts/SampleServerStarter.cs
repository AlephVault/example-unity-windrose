using AlephVault.Unity.Meetgard.Authoring.Behaviours.Server;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AlephVault.Unity.Meetgard.Scopes
{
    namespace Samples
    {
        [RequireComponent(typeof(NetworkServer))]
        public class SampleServerStarter : MonoBehaviour
        {
            [SerializeField]
            private KeyCode startKey = KeyCode.Z;

            [SerializeField]
            private KeyCode stopKey = KeyCode.X;

            private NetworkServer server;

            private void Awake()
            {
                server = GetComponent<NetworkServer>();
            }

            void Update()
            {
                if (Input.GetKeyDown(startKey) && !server.IsRunning && !server.IsListening)
                {
                    server.StartServer(9999);
                }

                if (Input.GetKeyDown(stopKey) && server.IsRunning && server.IsListening)
                {
                    server.StopServer();
                }
            }
        }
    }
}
