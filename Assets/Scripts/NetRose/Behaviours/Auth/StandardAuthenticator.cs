using System.Collections;
using UnityEngine;
using Mirror;

namespace NetRose
{
    namespace Behaviours
    {
        namespace Auth
        {
            /// <summary>
            ///   Allows login with just specifying the username and password. Usually, Two
            ///   connections logged with the same username will not be allowed, and the password
            ///   is matched against an encrypted version in some sort of database. This mode is
            ///   standard to most games that do not rely on social networking.
            /// </summary>
            public abstract class StandardAuthenticator : NetworkAuthenticator
            {
                static readonly ILogger logger = LogFactory.GetLogger(typeof(StandardAuthenticator));

                /// <summary>
                ///   The username to use as login. This field only makes sense for
                ///   clients and is not used at all in servers.
                /// </summary>
                public string Username;

                /// <summary>
                ///   The password to use as login. This field only makes sense for
                ///   clients and is not used at all in servers.
                /// </summary>
                public string Password;

                /// <summary>
                ///   An authentication request will hold a username and 
                ///   password, and will validate the login.
                /// </summary>
                public class AuthRequestMessage : MessageBase
                {
                    // use whatever credentials make sense for your game
                    // for example, you might want to pass the accessToken if using oauth
                    public string Username;
                    public string Password;
                }

                /// <summary>
                ///   The authentication response will hold one code and message.
                /// </summary>
                public class AuthResponseMessage : MessageBase
                {
                    public const byte SUCCESS = 200;
                    public byte code;
                    public string message;
                }

                public override void OnStartServer()
                {
                    // register a handler for the authentication request we expect from client
                    NetworkServer.RegisterHandler<AuthRequestMessage>(OnAuthRequestMessage, false);
                }

                public override void OnStartClient()
                {
                    // register a handler for the authentication response we expect from server
                    NetworkClient.RegisterHandler<AuthResponseMessage>(OnAuthResponseMessage, false);
                }

                public override void OnServerAuthenticate(NetworkConnection conn)
                {
                    // do nothing...wait for AuthRequestMessage from client
                }

                public override void OnClientAuthenticate(NetworkConnection conn)
                {
                    conn.Send(new AuthRequestMessage
                    {
                        Username = Username,
                        Password = Password
                    });
                }

                /// <summary>
                ///   Attempts a username/password authentication. This is intended for common games.
                /// </summary>
                /// <param name="username">The username to login with</param>
                /// <param name="password">The password to login with</param>
                /// <returns>The result, as an authentication response</returns>
                protected abstract AuthResponseMessage Authenticate(string username, string password);

                private void OnAuthRequestMessage(NetworkConnection conn, AuthRequestMessage msg)
                {
                    if (logger.LogEnabled()) logger.LogFormat(LogType.Log, "Authentication Request: {0} {1}", msg.Username, msg.Password);

                    AuthResponseMessage result = Authenticate(msg.Username, msg.Password);

                    conn.Send(result);

                    if (result.code == AuthResponseMessage.SUCCESS)
                    {
                        // Invoke the event to complete a successful authentication
                        OnServerAuthenticated.Invoke(conn);
                    }
                    else
                    {
                        // must set NetworkConnection isAuthenticated = false
                        conn.isAuthenticated = false;

                        // disconnect the client after 1 second so that response message gets delivered
                        StartCoroutine(DelayedDisconnect(conn, 1));
                    }
                }

                private IEnumerator DelayedDisconnect(NetworkConnection conn, float waitTime)
                {
                    yield return new WaitForSeconds(waitTime);
                    conn.Disconnect();
                }

                private void OnAuthResponseMessage(NetworkConnection conn, AuthResponseMessage msg)
                {
                    if (msg.code == 100)
                    {
                        if (logger.LogEnabled()) logger.LogFormat(LogType.Log, "Authentication Response: {0}", msg.message);

                        // Invoke the event to complete a successful authentication
                        OnClientAuthenticated.Invoke(conn);
                    }
                    else
                    {
                        logger.LogFormat(LogType.Error, "Authentication Response: {0}", msg.message);

                        // Set this on the client for local reference
                        conn.isAuthenticated = false;

                        // disconnect the client
                        conn.Disconnect();
                    }
                }
            }
        }
    }
}