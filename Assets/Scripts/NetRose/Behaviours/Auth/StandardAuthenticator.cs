using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

namespace NetRose
{
    namespace Behaviours
    {
        namespace Auth
        {
            /// <summary>
            ///   <para>
            ///     A standard authenticator involves several steps like:
            ///     - Building and sending an auth message to the server.
            ///     - Authenticating from the received auth message in server.
            ///       The result is a response, and a session object.
            ///   </para>
            /// </summary>
            /// <typeparam name="AuthMessage">The type of the auth message to send to the server to perform login</typeparam>
            /// <typeparam name="AccountID">The type of the id for the player's account</typeparam>
            /// <typeparam name="CharacterID">The type of the id for the player's characters</typeparam>
            /// <typeparam name="PreviewCharacterData">The type of the preview data for the player's characters</typeparam>
            /// <typeparam name="FullCharacterData">The type of the full data for the player's characters</typeparam>
            public abstract class StandardAuthenticator<AuthMessage, AccountID, CharacterID, PreviewCharacterData, FullCharacterData> : NetworkAuthenticator where AuthMessage : IMessageBase, new()
            {
                // The logger to use for these authenticators.
                private static readonly ILogger logger = LogFactory.GetLogger(typeof(StandardAuthenticator<AuthMessage, AccountID, CharacterID, PreviewCharacterData, FullCharacterData>));

                /// <summary>
                ///   This exception is thrown on auth/lookup-related errors.
                /// </summary>
                public class AccountException : Exception
                {
                    /// <summary>
                    ///   The error code to be serialized.
                    /// </summary>
                    public readonly string Code;

                    /// <summary>
                    ///   The error details to be serialized.
                    /// </summary>
                    public readonly Dictionary<string, string> Details;

                    public AccountException(string code, Dictionary<string, string> details)
                    {
                        Code = code;
                        Details = details == null ? new Dictionary<string, string>() : details;
                    }
                }

                /// <summary>
                ///   Throws an <see cref="AccountException"/> with the given details.
                /// </summary>
                /// <param name="code">The code for the thrown exception</param>
                /// <param name="details">More details of the thrown exception</param>
                protected void AccountError(string code, Dictionary<string, string> details)
                {
                    throw new AccountException(code, details);
                }

                /// <summary>
                ///   An authentication response has 3 fields: whether the request was
                ///     successful, its code, and more details.
                /// </summary>
                public class AuthResponse : MessageBase
                {
                    /// <summary>
                    ///   Whether the request was successful (i.e. the user successfully
                    ///     logged in).
                    /// </summary>
                    public bool IsSuccess = true;

                    /// <summary>
                    ///   A custom code, either for success or failure, for the status
                    ///     of the authentication attempt.
                    /// </summary>
                    public string Code = "success";

                    /// <summary>
                    ///   More optional details regarding the result of the login attempt.
                    /// </summary>
                    public Dictionary<string, string> Details = new Dictionary<string, string>();

                    /// <summary>
                    ///   Serializes all the fields of this message. The details are serialized
                    ///     as pairs of strings.
                    /// </summary>
                    /// <param name="writer">The writer to serialize this message into</param>
                    public override void Serialize(NetworkWriter writer)
                    {
                        writer.WriteBoolean(IsSuccess);
                        writer.WriteString(Code);
                        writer.WriteInt32(Details.Count);
                        foreach(KeyValuePair<string, string> pair in Details)
                        {
                            writer.WriteString(pair.Key);
                            writer.WriteString(pair.Value);
                        }
                    }

                    /// <summary>
                    ///   Deserializes all the fields of this message. The details are deserialized
                    ///     from pairs of strings.
                    /// </summary>
                    /// <param name="reader">The reader to deserialize this message from</param>
                    public override void Deserialize(NetworkReader reader)
                    {
                        IsSuccess = reader.ReadBoolean();
                        Code = reader.ReadString();
                        int count = reader.ReadInt32();
                        for(int index = 0; index < count; index++)
                        {
                            string key = reader.ReadString();
                            string value = reader.ReadString();
                            Details[key] = value;
                        }
                        base.Deserialize(reader);
                    }

                    /// <summary>
                    ///   Empty constructor for response objects.
                    /// </summary>
                    public AuthResponse() {}

                    /// <summary>
                    ///   Quick constructor for response objects.
                    /// </summary>
                    public AuthResponse(bool isSuccess, string code, Dictionary<string, string> details)
                    {
                        IsSuccess = isSuccess;
                        Code = code;
                        Details = details == null ? new Dictionary<string, string>() : details;
                    }
                }

                /// <summary>
                ///   Builds an authentication request from the data in the current object.
                /// </summary>
                /// <returns>The newly built message</returns>
                protected abstract AuthMessage BuildAuthMessage();

                /// <summary>
                ///   Tries to authenticate by using the given authentication message.
                /// </summary>
                /// <param name="request">The authentication message</param>
                /// <returns>The result of the auth process: an account id</returns>
                protected abstract AccountID Authenticate(AuthMessage request);

                /// <summary>
                ///   Tells whether the accounts support having several characters or
                ///     each account is its only character.
                /// </summary>
                protected abstract bool AllowsMultipleCharactersPerAccount();

                /// <summary>
                ///   Lists the characters for a given account ID. For single-character
                ///     games, the key will be <c>default(CharacterID)</c>, while for
                ///     multi-character (per account) games, no key will be such default
                ///     value. On error, this method should raise <see cref="AccountException"/>.
                /// </summary>
                /// <param name="accountId">The id of the account to get the characters from</param>
                /// <returns>A dictionary with the preview of available characters</returns>
                protected abstract Dictionary<CharacterID, PreviewCharacterData> ListCharacters(AccountID accountId);

                /// <summary>
                ///   Lists the characters for a given connection. For single-character
                ///     games, the key will be <c>default(CharacterID)</c>, while for
                ///     multi-character (per account) games, no key will be such default
                ///     value. On error, this method should raise <see cref="AccountException"/>.
                /// </summary>
                /// <param name="accountId">The connection of the account to get the characters from</param>
                /// <returns>A dictionary with the preview of available characters</returns>
                public Dictionary<CharacterID, PreviewCharacterData> ListCharacters(NetworkConnection connection)
                {
                    if (!connection.isAuthenticated || connection.authenticationData != null || ((AccountID)(connection.authenticationData)).Equals(default(AccountID)))
                    {
                        AccountError("authorization-required", null);
                    }
                    return ListCharacters(((AccountID)(connection.authenticationData)));
                }

                public override void OnStartServer()
                {
                    // Register a handler for the authentication request we expect from client.
                    NetworkServer.RegisterHandler<AuthMessage>(OnAuthRequestMessage, false);
                }

                public override void OnStartClient()
                {
                    // Register a handler for the authentication response we expect from server.
                    NetworkClient.RegisterHandler<AuthResponse>(OnAuthResponseMessage, false);
                }

                /// <summary>
                ///   When starting the authentication on client side, it
                ///     builds and sends the authentication request.
                /// </summary>
                /// <param name="conn">The connection to server to send the auth message to</param>
                public override void OnClientAuthenticate(NetworkConnection conn)
                {
                    conn.Send(BuildAuthMessage());
                }

                /// <summary>
                ///   When starting the authentication on server side, nothing
                ///     is expected to be done (however, subclasses may add more
                ///     behaviour here).
                /// </summary>
                /// <param name="conn">The connection to the client trying to login</param>
                public override void OnServerAuthenticate(NetworkConnection conn)
                {
                    // There is nothing to do here.
                }

                // Tries the authentication and, if success, the server-side workflow
                //   continues. On failure, it notifies and closes the connection.
                private void OnAuthRequestMessage(NetworkConnection conn, AuthMessage message)
                {
                    if (logger.LogEnabled()) logger.LogFormat(LogType.Log, "Authentication Request: {0}", message);

                    try
                    {
                        // Step 1.a: Tries to authenticate.
                        AccountID result = Authenticate(message);
                        // Step 1.b: Sends a success response to the client side.
                        conn.Send(new AuthResponse(true, "success", new Dictionary<string, string>()));
                        // Step 2: Succeeds authentication and continues the workflow.
                        //
                        // Stores the current session in the connection.
                        conn.authenticationData = result;
                        // Invoke the event to complete a successful authentication.
                        // It will also involve conn.isAuthenticated = true.
                        OnServerAuthenticated.Invoke(conn);
                    }
                    catch (AccountException e)
                    {
                        // Step 2: Fails authentication and aborts the workflow and connection.
                        //
                        // Clears isAuthenticated and session.
                        conn.isAuthenticated = false;
                        conn.authenticationData = null;
                        // Finally, disconnect the client after 1 second so that response message gets delivered.
                        StartCoroutine(DelayedDisconnect(conn, 1));
                    }
                }

                // This coroutine disconnects the client after one second.
                private IEnumerator DelayedDisconnect(NetworkConnection conn, float waitTime)
                {
                    yield return new WaitForSeconds(waitTime);
                    conn.Disconnect();
                }

                // Receives the authentication response. On success, it continues the
                //   client-side workflow.
                private void OnAuthResponseMessage(NetworkConnection conn, AuthResponse msg)
                {
                    if (msg.IsSuccess)
                    {
                        if (logger.LogEnabled()) logger.LogFormat(LogType.Log, "Authentication Response: {0}", msg.Code);

                        // Invoke the event to complete a successful authentication.
                        OnClientAuthenticated.Invoke(conn);
                    }
                    else
                    {
                        logger.LogFormat(LogType.Error, "Authentication Response: {0}", msg.Code);

                        // Clears isAuthenticated.
                        conn.isAuthenticated = false;
                        // Finally, disconnects the client immediately.
                        conn.Disconnect();
                    }
                }
            }
        }
    }
}
