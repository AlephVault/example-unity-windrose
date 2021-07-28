using System;
using UnityEngine;
using UnityEngine.UI;
using AlephVault.Unity.MMO.Authoring.Behaviours.Authentication;
using AlephVault.Unity.MMO.Types;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AlephVault.Unity.MMO.Samples
{
    namespace Behaviours
    {
        namespace UI
        {
            /// <summary>
            ///   A dummy authenticator with username/password which
            ///   has a serialized dictionary as source. There is
            ///   a single authentication (a single realm, and a
            ///   single authentication method).
            /// </summary>
            [RequireComponent(typeof(Authenticator))]
            public class DummyAuthenticator : MonoBehaviour
            {
                private const string DummyLogin = "DummyLogin";

                /// <summary>
                ///   This class contains a dummy user => password
                ///   authentication base.
                /// </summary>
                [Serializable]
                private class UserPasswordDictionary : Support.Generic.Authoring.Types.Dictionary<string, string> {}

#if UNITY_EDITOR
                [CustomPropertyDrawer(typeof(UserPasswordDictionary))]
                public class UserPasswordDictionaryDrawer : Support.Generic.Authoring.Types.DictionaryPropertyDrawer {}
#endif

                /// <summary>
                ///   Some dumb users and passwords will be added
                ///   here to test the authentication.
                /// </summary>
                [SerializeField]
                private UserPasswordDictionary authDB = new UserPasswordDictionary();

                private Authenticator authenticator;

                private void Awake()
                {
                    authenticator = GetComponent<Authenticator>();
                }

                private void Start()
                {
                    authenticator.OnAccountLoginOK += Authenticator_OnAccountLoginOK;
                    authenticator.OnAccountLoginFailed += Authenticator_OnAccountLoginFailed;
                    authenticator.OnAuthenticationAlreadyDone += Authenticator_OnAuthenticationAlreadyDone;
                    authenticator.OnAuthenticationOK += Authenticator_OnAuthenticationOK;
                    authenticator.OnAuthenticationFailed += Authenticator_OnAuthenticationFailed;
                    authenticator.OnAuthenticationTimeout += Authenticator_OnAuthenticationTimeout;
                    RegisterLoginMethod();
                }

                private void Authenticator_OnAccountLoginOK(ulong connectionId, Response response, Authenticator.AccountId accountId)
                {
                    Debug.LogFormat("Server side: Login success for account id {0} in realm {1}", accountId);
                }

                private void Authenticator_OnAccountLoginFailed(ulong connectionId, Response response, Authenticator.AccountId accountId)
                {
                    Debug.LogFormat("Server side: Login failed for account id {0} in realm {1}", accountId.Item1, accountId.Item2);
                }

                private void Authenticator_OnAuthenticationAlreadyDone()
                {
                    Debug.LogFormat("Client side: Already authenticated");
                }

                private void Authenticator_OnAuthenticationOK(Response obj)
                {
                    Debug.LogFormat("Client side: Authentication success");
                }

                private void Authenticator_OnAuthenticationFailed(Response obj)
                {
                    Debug.LogFormat("Client side: Authentication failed");
                }

                private void Authenticator_OnAuthenticationTimeout()
                {
                    Debug.LogFormat("Client side: Authentication timeout");
                }

                private void OnDestroy()
                {
                    authenticator.OnAccountLoginOK -= Authenticator_OnAccountLoginOK;
                    authenticator.OnAccountLoginFailed -= Authenticator_OnAccountLoginFailed;
                    authenticator.OnAuthenticationAlreadyDone -= Authenticator_OnAuthenticationAlreadyDone;
                    authenticator.OnAuthenticationOK -= Authenticator_OnAuthenticationOK;
                    authenticator.OnAuthenticationFailed -= Authenticator_OnAuthenticationFailed;
                    authenticator.OnAuthenticationTimeout -= Authenticator_OnAuthenticationTimeout;
                }

                private void RegisterLoginMethod()
                {
                    authenticator.RegisterLoginMethod(DummyLogin, async (reader) =>
                    {
                        string username = reader.ReadString().ToString();
                        string password = reader.ReadString().ToString();
                        try
                        {
                            string expectedPassword = authDB[username];
                            if (expectedPassword == password)
                            {
                                return new Tuple<Response, Authenticator.AccountId>(new Response() { Success = true, Code = "OK" }, new Authenticator.AccountId(username, "dummy"));
                            }
                            else
                            {
                                return new Tuple<Response, Authenticator.AccountId>(new Response() { Success = false, Code = "Mismatch" }, new Authenticator.AccountId(username, "dummy"));
                            }
                        }
                        catch(KeyNotFoundException)
                        {
                            return new Tuple<Response, Authenticator.AccountId>(new Response() { Success = false, Code = "Unknown" }, new Authenticator.AccountId(username, "dummy"));
                        }
                    });
                }

                public void DumbAuthenticate(string username, string password)
                {
                    authenticator.Authenticate(DummyLogin, (writer) =>
                    {
                        writer.WriteString(username);
                        writer.WriteString(password);
                    });
                }
            }
        }
    }
}
