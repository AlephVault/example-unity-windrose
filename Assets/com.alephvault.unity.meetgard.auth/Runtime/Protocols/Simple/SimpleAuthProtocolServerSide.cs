using AlephVault.Unity.Binary;
using AlephVault.Unity.Meetgard.Auth.Types;
using AlephVault.Unity.Meetgard.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace AlephVault.Unity.Meetgard.Auth
{
    namespace Protocols
    {
        namespace Simple
        {
            /// <summary>
            ///   This is the server-side implementation of a simple
            ///   authentication protocol. The same server doing the
            ///   authentication, is the server mounting the game.
            ///   This client is the counterpart and connects to a
            ///   single server (it may be used in more complex
            ///   login interactions, though).
            /// </summary>
            /// <typeparam name="Definition">A subclass of <see cref="SimpleAuthProtocolDefinition{LoginOK, LoginFailed, Kicked}"/></typeparam>
            /// <typeparam name="LoginOK">The type of the "successful login" message</typeparam>
            /// <typeparam name="LoginFailed">The type of the "failed login" message</typeparam>
            /// <typeparam name="Kicked">The type of the "kicked" message</typeparam>
            /// <typeparam name="AccountIDType">The type of the account id</typeparam>
            public abstract partial class SimpleAuthProtocolServerSide<
                Definition, LoginOK, LoginFailed, Kicked,
                AccountIDType, AccountPreviewDataType, AccountDataType
            > : ProtocolServerSide<Definition>
                where LoginOK : ISerializable, new()
                where LoginFailed : ISerializable, new()
                where Kicked : IKickMessage<Kicked>, new()
                where AccountPreviewDataType : ISerializable, new()
                where AccountDataType : IRecordWithPreview<AccountIDType, AccountPreviewDataType>
                where Definition : SimpleAuthProtocolDefinition<LoginOK, LoginFailed, Kicked>, new()
            {
                /// <summary>
                ///   Typically, in this Start callback function
                ///   all the Send* shortcuts will be instantiated.
                /// </summary>
                protected void Start()
                {
                    MakeSenders();
                }

                protected override void SetIncomingMessageHandlers()
                {
                    SetLoginMessageHandlers();
                    AddIncomingMessageHandler("Logout", async (proto, clientId) =>
                    {
                        // TODO implement Logout.
                    });
                }

                /// <summary>
                ///   Implement this method with several calls to
                ///   <see cref="AddLoginMessageHandler{T}(string)"/>,
                ///   each one for each allowed login method.
                /// </summary>
                protected abstract void SetLoginMessageHandlers();

                /// <summary>
                ///   <para>
                ///     Adds a login handler for a specific method type.
                ///     The login handler returns a tuple with 3 elements:
                ///     whether the login was successful, the login success
                ///     message (or null if it was successful) and the login
                ///     failure message (or null if it was NOT successful).
                ///     As a fourth parameter, the account id will be given
                ///     (or its default value) when the login is successful.
                ///   </para>
                ///   <para>
                ///     The login handler is responsible of logging. In
                ///     case of success, the session will start for that
                ///     account (each session type is differently handled),
                ///     which will be implemented in a different component.
                ///   </para>
                /// </summary>
                /// <typeparam name="T">The type of the login meesage</typeparam>
                /// <param name="method">The name of the method to use</param>
                /// <param name="handler">The handler to use to perform the login</param>
                protected void AddLoginMessageHandler<T>(string method, Func<T, Task<Tuple<bool, LoginOK, LoginFailed, AccountIDType>>> doLogin) where T : ISerializable, new()
                {
                    AddIncomingMessageHandler<T>("Login:" + method, async (proto, clientId, message) => {
                        // 1. Receive the message.
                        // 2. Process the message.
                        // 3. On success: trigger the success.
                        // 4. On failure: trigger the failure.
                        Tuple<bool, LoginOK, LoginFailed, AccountIDType> result = await doLogin(message);
                        if (result.Item1)
                        {
                            await SendLoginOK(clientId, result.Item2);
                            await (OnSessionStarted?.Invoke(clientId, result.Item4) ?? Task.CompletedTask);
                        }
                        else
                        {
                            await SendLoginFailed(clientId, result.Item3);
                        }
                    });
                }

                /// <summary>
                ///   This event is triggered when the login is successful.
                ///   The connection id and also the id of the account that
                ///   successfully logged in are given as arguments.
                /// </summary>
                public event Func<ulong, AccountIDType, Task> OnSessionStarted = null;

                /// <summary>
                ///   This event is triggered when a user is logged out,
                ///   be it gracefully or kicked. On graceful logout, the
                ///   third argument will be null.
                /// </summary>
                public event Func<ulong, AccountIDType, Kicked, Task> OnSessionEnded = null;

                /// <summary>
                ///   A single method must be given here. Such method must
                ///   try a kick of a given active session by its account
                ///   id (one single active session at most can exist for
                ///   a given account).
                /// </summary>
                public event Func<AccountIDType, Kicked, Task<bool>> OnKickByAccountIDRequested = null;

                /// <summary>
                ///   A single method must be given here. Such method must
                ///   try a kick of a given active session by its connection
                ///   id (one single active session at most can exist for
                ///   a given connection).
                /// </summary>
                public event Func<AccountIDType, Kicked, Task<bool>> OnKickByConnectionIDRequested = null;


            }
        }
    }
}