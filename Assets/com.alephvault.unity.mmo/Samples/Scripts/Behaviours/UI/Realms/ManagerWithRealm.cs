using AlephVault.Unity.MMO.Authoring.Behaviours.Authentication;
using AlephVault.Unity.MMO.Samples.Behaviours.Realms;
using MLAPI;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace AlephVault.Unity.MMO.Samples
{
    namespace Behaviours
    {
        namespace UI
        {
            namespace Realms
            {
                public class ManagerWithRealm : MonoBehaviour
                {
                    private Button startClient;
                    private Button startHost;
                    private Button startServer;
                    private Button stopClient;
                    private Button stopHost;
                    private Button stopServer;
                    private InputField usernameField;
                    private InputField passwordField;

                    [SerializeField]
                    private float secondsToWait = 2f;

                    void Start()
                    {
                        foreach (Button button in GetComponentsInChildren<Button>())
                        {
                            switch (button.gameObject.name)
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
                            usernameField = transform.Find("Username").GetComponent<InputField>();
                            passwordField = transform.Find("Password").GetComponent<InputField>();
                        }
                        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
                        NetworkManager.Singleton.GetComponent<Authenticator>().OnAuthenticationOK += ManagerWithRealm_OnAuthenticationOK;
                        NetworkManager.Singleton.GetComponent<Authenticator>().OnAuthenticationFailed += ManagerWithRealm_OnAuthenticationFailed;
                        NetworkManager.Singleton.GetComponent<Authenticator>().OnAuthenticationEnded += ManagerWithRealm_OnAuthenticationEnded;
                        NetworkManager.Singleton.GetComponent<Authenticator>().OnAuthenticationAlreadyDone += ManagerWithRealm_OnAuthenticationAlreadyDone;
                        NetworkManager.Singleton.GetComponent<Authenticator>().OnAuthenticationTimeout += ManagerWithRealm_OnAuthenticationTimeout;
                    }

                    private void OnDestroy()
                    {
                        NetworkManager.Singleton.GetComponent<Authenticator>().OnAuthenticationOK -= ManagerWithRealm_OnAuthenticationOK;
                        NetworkManager.Singleton.GetComponent<Authenticator>().OnAuthenticationFailed -= ManagerWithRealm_OnAuthenticationFailed;
                        NetworkManager.Singleton.GetComponent<Authenticator>().OnAuthenticationEnded -= ManagerWithRealm_OnAuthenticationEnded;
                        NetworkManager.Singleton.GetComponent<Authenticator>().OnAuthenticationAlreadyDone -= ManagerWithRealm_OnAuthenticationAlreadyDone;
                        NetworkManager.Singleton.GetComponent<Authenticator>().OnAuthenticationTimeout -= ManagerWithRealm_OnAuthenticationTimeout;
                    }

                    private void ManagerWithRealm_OnAuthenticationTimeout()
                    {
                        Debug.Log(">>> Authentication client: Timeout");
                    }

                    private void ManagerWithRealm_OnAuthenticationAlreadyDone()
                    {
                        Debug.Log(">>> Authentication client: Already authenticated");
                    }

                    private void ManagerWithRealm_OnAuthenticationEnded(Types.Reason obj)
                    {
                        Debug.Log(">>> Authentication client: Closed");
                    }

                    private void ManagerWithRealm_OnAuthenticationFailed(Types.Response obj)
                    {
                        Debug.Log(">>> Authentication client: Failed");
                    }

                    private void ManagerWithRealm_OnAuthenticationOK(Types.Response obj)
                    {
                        Debug.Log(">>> Authentication client: Success");
                    }

                    private void OnClientConnected(ulong obj)
                    {
                        if (NetworkManager.Singleton.IsClient)
                        {
                            StartCoroutine(WaitAndLogin());
                        }
                    }

                    private IEnumerator WaitAndLogin()
                    {
                        yield return new WaitForSeconds(secondsToWait);
                        NetworkManager.Singleton.GetComponent<ChatRealm>().DummyLogin(usernameField.text, passwordField.text);
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
}
