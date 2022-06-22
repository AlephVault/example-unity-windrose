using System;
using AlephVault.Unity.Meetgard.Authoring.Behaviours.Server;
using UnityEngine;

namespace AlephVault.Unity.EVMGames.Auth
{
    namespace Samples
    {
        [RequireComponent(typeof(NetworkServer))]
        public class SampleServerLauncher : MonoBehaviour
        {
            public ushort Port;

            private void Start()
            {
                GetComponent<NetworkServer>().StartServer(Port);
            }

            private void OnDestroy()
            {
                GetComponent<NetworkServer>().StopServer();
            }
        }
    }
}