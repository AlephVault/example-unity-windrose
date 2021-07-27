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
            public class DummyAuthenticator : Authenticator
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

                // List of the active sessions.
                private Dictionary<ulong, string> usersLoggedIn = new Dictionary<ulong, string>();

                // Checks whether a session is active for a connection.
                public override bool IsLoggedIn(ulong clientId)
                {
                    return usersLoggedIn.ContainsKey(clientId);
                }

                protected override void OnAccountLoginFailed(ulong clientId, Response response, object accountId, object realm)
                {
                    Debug.LogFormat("Server side: Login failed for account id {0} in realm {1}", accountId, realm);
                }

                protected override void OnAccountLoginOK(ulong clientId, Response response, object accountId, string realm, object forcerAccountId, string forcerRealm)
                {
                    // Only one realm will exist, and that realm uses a string account id.
                    usersLoggedIn.Add(clientId, accountId as string);
                    Debug.LogFormat("Server side: Login success for account id {0} in realm {1}", accountId, realm);
                }

                protected override void OnAuthenticationAlreadyDone()
                {
                    Debug.LogFormat("Client side: Already authenticated");
                }

                protected override void OnAuthenticationFailure(Response response)
                {
                    Debug.LogFormat("Client side: Authentication failed");
                }

                protected override void OnAuthenticationOK(Response response)
                {
                    Debug.LogFormat("Client side: Authentication success");
                }

                protected override void OnAuthenticationTimeout()
                {
                    Debug.LogFormat("Client side: Authentication timeout");
                }

                protected override void RegisterLoginMethods()
                {
                    RegisterLoginMethod(DummyLogin, async (reader) =>
                    {
                        string username = reader.ReadString().ToString();
                        string password = reader.ReadString().ToString();
                        try
                        {
                            string expectedPassword = authDB[username];
                            if (expectedPassword == password)
                            {
                                return new Tuple<Response, object, string>(new Response() { Success = true, Code = "OK" }, username, "dummy");
                            }
                            else
                            {
                                return new Tuple<Response, object, string>(new Response() { Success = false, Code = "Mismatch" }, username, "dummy");
                            }
                        }
                        catch(KeyNotFoundException)
                        {
                            return new Tuple<Response, object, string>(new Response() { Success = false, Code = "Unknown" }, null, "dummy");
                        }
                    });
                }

                public void DumbAuthenticate(string username, string password)
                {
                    Authenticate(DummyLogin, (writer) =>
                    {
                        writer.WriteString(username);
                        writer.WriteString(password);
                    });
                }

                protected override void ClientSetup(ulong clientId)
                {
                    Debug.LogFormat("Setting client {0} up", clientId);
                }

                protected override void ClientTeardown(ulong clientId)
                {
                    Debug.LogFormat("Tearing client {0} down", clientId);
                    usersLoggedIn.Remove(clientId);
                }
            }
        }
    }
}
