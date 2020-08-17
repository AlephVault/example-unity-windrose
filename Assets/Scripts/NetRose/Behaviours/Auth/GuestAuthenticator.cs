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
            ///   Allows login with just specifying the username. Usually, Two connections
            ///   logged with the same username will not be allowed, and the username can be
            ///   be restricted to a particular format. This mode is mostly used in minigames
            ///   and not intended for serious gaming.
            /// </summary>
            public abstract class GuestAuthenticator : NetworkAuthenticator
            {
                static readonly ILogger logger = LogFactory.GetLogger(typeof(GuestAuthenticator));

                /// <summary>
                ///   The username to use as login. This field only makes sense for
                ///   clients and is not used at all in servers.
                /// </summary>
                public string Username;

                /// <summary>
                ///   An authentication request will hold a username and will validate
                ///   its format and whether it is currently in use or not.
                /// </summary>
                public class AuthRequestMessage : MessageBase
                {
                    public string Username;
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
                    });
                }

                /// <summary>
                ///   Attempts a guest authentication. This is intended for minigames.
                /// </summary>
                /// <param name="username">The username to login as</param>
                /// <returns>The result, as an authentication response</returns>
                protected abstract AuthResponseMessage Authenticate(string username);

                private void OnAuthRequestMessage(NetworkConnection conn, AuthRequestMessage msg)
                {
                    if (logger.LogEnabled()) logger.LogFormat(LogType.Log, "Authentication Request: {0}", msg.Username);

                    AuthResponseMessage result = Authenticate(msg.Username);

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
