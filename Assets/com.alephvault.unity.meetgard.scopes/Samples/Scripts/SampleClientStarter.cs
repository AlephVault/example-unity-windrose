using AlephVault.Unity.Meetgard.Authoring.Behaviours.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AlephVault.Unity.Meetgard.Scopes
{
    namespace Samples
    {
        [RequireComponent(typeof(SampleProtocolClientSide))]
        public class SampleClientStarter : MonoBehaviour
        {
            [SerializeField]
            private KeyCode startKey = KeyCode.A;

            [SerializeField]
            private KeyCode stopKey = KeyCode.S;

            [SerializeField]
            private KeyCode[] gotoDefaultKeys;

            [SerializeField]
            private KeyCode gotoLimboKey = KeyCode.D;

            [SerializeField]
            private KeyCode gotoExtraKey = KeyCode.F;

            private NetworkClient client;
            private SampleProtocolClientSide protocol;

            private void Awake()
            {
                client = GetComponent<NetworkClient>();
                protocol = GetComponent<SampleProtocolClientSide>();
            }

            void Update()
            {
                if (Input.GetKeyDown(startKey) && !client.IsRunning && !client.IsConnected)
                {
                    client.Connect("localhost", 9999);
                }

                if (client.IsRunning && client.IsConnected)
                {
                    if (Input.GetKeyDown(stopKey))
                    {
                        client.Close();
                    }
                    else if (Input.GetKeyDown(gotoLimboKey))
                    {
                        protocol.DoSendGoToLimbo();
                    }
                    else if (Input.GetKeyDown(gotoExtraKey))
                    {
                        protocol.DoSendGoToExtra();
                    }
                    else
                    {
                        for(uint index = 0; index < gotoDefaultKeys.Length; index++)
                        {
                            if (Input.GetKeyDown(gotoDefaultKeys[index]))
                            {
                                protocol.DoSendGoToDefault(index + 1);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}