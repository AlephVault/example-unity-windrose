using MLAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AlephVault.Unity.MMO.Samples
{
    namespace Behaviours
    {
        namespace UI
        {
            public class Manager : MonoBehaviour
            {
                private Button startClient;
                private Button startHost;
                private Button startServer;
                private Button stopClient;
                private Button stopHost;
                private Button stopServer;

                void Awake()
                {
                    foreach(Button button in GetComponentsInChildren<Button>())
                    {
                        switch(button.gameObject.name)
                        {
                            case "StartClient":
                                startClient = button;
                                startClient.onClick.AddListener(() =>
                                {
                                    Debug.Log("Starting client...");
                                    NetworkManager.Singleton.StartClient();
                                });
                                break;
                            case "StartHost":
                                startHost = button;
                                startHost.onClick.AddListener(() =>
                                {
                                    Debug.Log("Starting server in host mode...");
                                    NetworkManager.Singleton.StartHost();
                                });
                                break;
                            case "StartServer":
                                startServer = button;
                                startServer.onClick.AddListener(() =>
                                {
                                    Debug.Log("Starting dedicated server...");
                                    NetworkManager.Singleton.StartServer();
                                });
                                break;
                            case "StopClient":
                                stopClient = button;
                                stopClient.onClick.AddListener(() =>
                                {
                                    Debug.Log("Stopping client...");
                                    NetworkManager.Singleton.StopClient();
                                });
                                break;
                            case "StopHost":
                                stopHost = button;
                                stopHost.onClick.AddListener(() =>
                                {
                                    Debug.Log("Stopping server in host mode...");
                                    NetworkManager.Singleton.StopHost();
                                });
                                break;
                            case "StopServer":
                                stopServer = button;
                                stopServer.onClick.AddListener(() =>
                                {
                                    Debug.Log("Starting dedicated server...");
                                    NetworkManager.Singleton.StopServer();
                                });
                                break;
                        }
                    }
                }

                private void Update()
                {
                    bool singleton = NetworkManager.Singleton != null;
                    bool client = singleton && NetworkManager.Singleton.IsClient;
                    bool server = singleton && NetworkManager.Singleton.IsServer;
                    bool connected = singleton && NetworkManager.Singleton.IsListening;
                    bool clientOnly = client && !server;
                    bool host = client && server;
                    bool serverOnly = !client && server;
                    startClient.interactable = !connected;
                    startHost.interactable = !connected;
                    startServer.interactable = !connected;
                    stopServer.interactable = serverOnly;
                    stopHost.interactable = host;
                    stopClient.interactable = clientOnly;
                }
            }
        }
    }
}
