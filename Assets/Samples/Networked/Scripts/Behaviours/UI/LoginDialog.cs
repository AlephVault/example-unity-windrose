using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using Mirror;
using NetworkedSamples.Behaviours.Sessions;

namespace NetworkedSamples
{
    namespace Behaviours
    {
        public class LoginDialog : MonoBehaviour
        {
            // Use this for initialization
            void Start()
            {
                transform.Find("Login").GetComponent<Button>().onClick.AddListener(() =>
                {
                    string username = transform.Find("Username").GetComponent<TMP_InputField>().text.Trim();
                    string password = transform.Find("Password").GetComponent<TMP_InputField>().text;
                    NetworkManager manager = NetworkManager.singleton;
                    SampleMessageForwarder forwarder = manager.GetComponent<SampleMessageForwarder>();
                    SampleAuthenticator authenticator = manager.GetComponent<SampleAuthenticator>();
                    if (username == "" || password == "")
                    {
                        forwarder.onMessage.Invoke("Username/Password must both be set.");
                    }
                    else
                    {
                        // Starts a client connection to the default port (e.g. 7777) and address (e.g. localhost).
                        // It will also try authenticating.
                        authenticator.Username = username;
                        authenticator.Password = password;
                        manager.StartClient();
                    }
                });
            }
        }
    }
}