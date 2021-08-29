using AlephVault.Unity.Binary;
using AlephVault.Unity.Meetgard.Auth.Types;
using AlephVault.Unity.Meetgard.Protocols;
using AlephVault.Unity.Meetgard.Server;
using System;
using System.Threading.Tasks;

namespace AlephVault.Unity.Meetgard.Auth
{
    namespace Protocols
    {
        namespace Simple
        {
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
                //
                //
                //
                // Wrappers are functions that wrap a typed or untyped handler
                // with specific behaviour. Two of them are supported so far:
                //
                //
                //

                // Wraps a permission checker function to invoke the wrapped
                // function and catch any error.
                private Func<ulong, Task<bool>> WrapAllowedCheck(Func<ulong, Task<bool>> allowed)
                {
                    return async (clientId) =>
                    {
                        try
                        {
                            return await allowed(clientId);
                        }
                        catch (Exception e)
                        {
                            try
                            {
                                await OnSessionError(clientId, SessionStage.PermissionCheck, e);
                            }
                            catch { /* Diaper pattern - intentional */ }
                            return false;
                        }
                    };
                }

                // In order to require logout, the client must be login pending.
                // This means that the client must be in the concurrent set of
                // pending connections.
                private bool IsLoginPending(ulong clientId)
                {
                    // TODO implement.
                    return true;
                }

                /// <summary>
                ///   Wraps a handler to require the client to be logged in.
                ///   This is related to a custom target definition type and
                ///   protocol server side type.
                /// </summary>
                /// <typeparam name="TargetDefinitionType">The related protocol definition type</typeparam>
                /// <typeparam name="TargetProtocolType">The related protocol server side type</typeparam>
                /// <param name="allowed">The callback to check whether the client is allowed to perform the action</param>
                /// <param name="handler">The message handler</param>
                /// <returns>The wrapped message handler</returns>
                public Func<TargetProtocolType, ulong, Task> LoginRequired<TargetDefinitionType, TargetProtocolType>(Func<ulong, Task<bool>> allowed, Func<TargetProtocolType, ulong, Task> handler)
                    where TargetDefinitionType : ProtocolDefinition, new()
                    where TargetProtocolType : ProtocolServerSide<TargetDefinitionType>
                {
                    Func<ulong, Task<bool>> isAllowed = WrapAllowedCheck(allowed);
                    return async (proto, clientId) =>
                    {
                        if (SessionExists(clientId))
                        {
                            if (await isAllowed(clientId))
                            {
                                await handler(proto, clientId);
                            }
                            else
                            {
                                await SendForbidden(clientId);
                            }
                        }
                        else
                        {
                            await SendNotLoggedIn(clientId);
                        }
                    };
                }

                /// <summary>
                ///   Wraps a handler to require the client to be logged in.
                ///   This is related to a custom target definition type and
                ///   protocol server side type.
                /// </summary>
                /// <typeparam name="TargetDefinitionType">The related protocol definition type</typeparam>
                /// <typeparam name="TargetProtocolType">The related protocol server side type</typeparam>
                /// <typeparam name="T">The message type</typeparam>
                /// <param name="allowed">The callback to check whether the client is allowed to perform the action</param>
                /// <param name="handler">The message handler</param>
                /// <returns>The wrapped message handler</returns>
                public Func<TargetProtocolType, ulong, T, Task> LoginRequired<TargetDefinitionType, TargetProtocolType, T>(Func<ulong, Task<bool>> allowed, Func<TargetProtocolType, ulong, T, Task> handler)
                    where T : ISerializable, new()
                    where TargetDefinitionType : ProtocolDefinition, new()
                    where TargetProtocolType : ProtocolServerSide<TargetDefinitionType>
                {
                    Func<ulong, Task<bool>> isAllowed = WrapAllowedCheck(allowed);
                    return async (proto, clientId, message) =>
                    {
                        if (SessionExists(clientId))
                        {
                            if (await isAllowed(clientId))
                            {
                                await handler(proto, clientId, message);
                            }
                            else
                            {
                                await SendForbidden(clientId);
                            }
                        }
                        else
                        {
                            await SendNotLoggedIn(clientId);
                        }
                    };
                }

                /// <summary>
                ///   Wraps a handler to require the client to be logged in.
                ///   This is related to a custom target definition type and
                ///   protocol server side type.
                /// </summary>
                /// <typeparam name="TargetDefinitionType">The related protocol definition type</typeparam>
                /// <typeparam name="TargetProtocolType">The related protocol server side type</typeparam>
                /// <param name="handler">The message handler</param>
                /// <returns>The wrapped message handler</returns>
                public Func<TargetProtocolType, ulong, Task> LoginRequired<TargetDefinitionType, TargetProtocolType>(Func<TargetProtocolType, ulong, Task> handler)
                    where TargetDefinitionType : ProtocolDefinition, new()
                    where TargetProtocolType : ProtocolServerSide<TargetDefinitionType>
                {
                    return async (proto, clientId) =>
                    {
                        if (SessionExists(clientId))
                        {
                            await SendForbidden(clientId);
                        }
                        else
                        {
                            await SendNotLoggedIn(clientId);
                        }
                    };
                }

                /// <summary>
                ///   Wraps a handler to require the client to be logged in.
                ///   This is related to a custom target definition type and
                ///   protocol server side type.
                /// </summary>
                /// <typeparam name="TargetDefinitionType">The related protocol definition type</typeparam>
                /// <typeparam name="TargetProtocolType">The related protocol server side type</typeparam>
                /// <typeparam name="T">The message type</typeparam>
                /// <param name="handler">The message handler</param>
                /// <returns>The wrapped message handler</returns>
                public Func<TargetProtocolType, ulong, T, Task> LoginRequired<TargetDefinitionType, TargetProtocolType, T>(Func<TargetProtocolType, ulong, T, Task> handler)
                    where T : ISerializable, new()
                    where TargetDefinitionType : ProtocolDefinition, new()
                    where TargetProtocolType : ProtocolServerSide<TargetDefinitionType>
                {
                    return async (proto, clientId, message) =>
                    {
                        if (SessionExists(clientId))
                        {
                            await handler(proto, clientId, message);
                        }
                        else
                        {
                            await SendNotLoggedIn(clientId);
                        }
                    };
                }

                /// <summary>
                ///   Wraps a handler to require the client to be logged out.
                ///   This is related to a custom target definition type and
                ///   protocol server side type.
                /// </summary>
                /// <typeparam name="TargetDefinitionType">The related protocol definition type</typeparam>
                /// <typeparam name="TargetProtocolType">The related protocol server side type</typeparam>
                /// <param name="handler">The message handler</param>
                /// <returns>The wrapped message handler</returns>
                public Func<TargetProtocolType, ulong, Task> LogoutRequired<TargetDefinitionType, TargetProtocolType>(Func<TargetProtocolType, ulong, Task> handler)
                    where TargetDefinitionType : ProtocolDefinition, new()
                    where TargetProtocolType : ProtocolServerSide<TargetDefinitionType>
                {
                    return async (proto, clientId) =>
                    {
                        if (IsLoginPending(clientId))
                        {
                            await handler(proto, clientId);
                        }
                        else
                        {
                            await SendAlreadyLoggedIn(clientId);
                        }
                    };
                }

                /// <summary>
                ///   Wraps a handler to require the client to be logged out.
                ///   This is related to a custom target definition type and
                ///   protocol server side type.
                /// </summary>
                /// <typeparam name="TargetDefinitionType">The related protocol definition type</typeparam>
                /// <typeparam name="TargetProtocolType">The related protocol server side type</typeparam>
                /// <typeparam name="T">The message type</typeparam>
                /// <param name="handler">The message handler</param>
                /// <returns>The wrapped message handler</returns>
                public Func<TargetProtocolType, ulong, T, Task> LogoutRequired<TargetDefinitionType, TargetProtocolType, T>(Func<TargetProtocolType, ulong, T, Task> handler)
                    where T : ISerializable, new()
                    where TargetDefinitionType : ProtocolDefinition, new()
                    where TargetProtocolType : ProtocolServerSide<TargetDefinitionType>
                {
                    return async (proto, clientId, message) =>
                    {
                        if (IsLoginPending(clientId))
                        {
                            await handler(proto, clientId, message);
                        }
                        else
                        {
                            await SendAlreadyLoggedIn(clientId);
                        }
                    };
                }
            }
        }
    }
}