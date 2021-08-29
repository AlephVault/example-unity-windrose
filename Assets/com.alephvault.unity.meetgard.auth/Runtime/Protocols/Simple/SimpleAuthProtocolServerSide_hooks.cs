using AlephVault.Unity.Binary;
using AlephVault.Unity.Meetgard.Auth.Types;
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
                /// <summary>
                ///   Tells when an account was not found by its ID.
                /// </summary>
                public class AccountNotFound : Exception {
                    public readonly AccountIDType ID;

                    public AccountNotFound(AccountIDType id) : base() { ID = id; }
                    public AccountNotFound(AccountIDType id, string message) : base(message) { ID = id; }
                    public AccountNotFound(AccountIDType id, string message, Exception cause) : base(message, cause) { ID = id; }
                }

                /// <summary>
                ///   The current stage of the session. This stage
                ///   involves load / unload session operations,
                ///   and not the gameplay itself.
                /// </summary>
                protected enum SessionStage
                {
                    AccountLoad,
                    Initialization,
                    PermissionCheck,
                    Termination
                }

                //
                //
                //
                // Session hooks start here. They are related to loading
                // account data and preparing the session appropriately,
                // and also what to do when the session was told to
                // terminate (logout of kick).
                //
                //
                //

                // This function is invoked when a client was successfully
                // logged in. The full account data will be prepared and
                // the session will start. An unhandled error will become
                // a deal breaker and the client will be kicked and told
                // it was due to an unexpected error.
                private async Task OnLoggedIn(ulong clientId, AccountIDType accountId)
                {
                    AccountDataType accountData = default;
                    // 1. Get the account data.
                    // 2. On error:
                    //   2.1. Handle the error appropriately.
                    //   2.2. Send a kick message with "unexpected error on account load".
                    //   2.3. Close the connection.
                    try
                    {
                        accountData = await FindAccount(accountId);
                        if (accountData.Equals(default(AccountDataType)))
                        {
                            throw new AccountNotFound(accountId);
                        }
                    }
                    catch(Exception e)
                    {
                        try
                        {
                            await OnSessionError(clientId, SessionStage.AccountLoad, e);
                        }
                        catch { /* Diaper pattern - intentional */ }
                        await SendKicked(clientId, new Kicked().WithAccountLoadErrorReason());
                        server.Close(clientId);
                    }
                    // 3. Add the session.
                    // 4. Invoke the "session initializing" hook, considering account data.
                    // 5. On error:
                    //   5.1. Handle the error appropriately.
                    //   5.2. Send a kick message with "unexpected error on session start".
                    //   5.3. Close the connection.
                    AddSession(clientId, accountId);
                    try
                    {
                        await OnSessionStarting(clientId, accountData);
                    }
                    catch(Exception e)
                    {
                        try
                        {
                            await OnSessionError(clientId, SessionStage.Initialization, e);
                        }
                        catch { /* Diaper pattern - intentional */ }
                        await SendKicked(clientId, new Kicked().WithSessionInitializationErrorReason());
                        server.Close(clientId);
                    }
                }

                // This function is invoked when a client was logged out due
                // to a logout or kick command, and the session was already
                // established and loaded before that. The session must end
                // and the connection must be closed.
                private async Task OnLoggedOut(ulong clientId, Kicked reason)
                {
                    // 1. The session will still exist.
                    //   1.1. But the client already received the kick/logged-out message.
                    //   1.2. "reason" will be default(Kicked) on graceful logout.
                    // 2. Invoke the "session terminating" hook, considering kick reason.
                    // 3. On error:
                    //   3.1. Handle the error appropriately.
                    // 4. Remove the session.
                    // 5. Close the connection.
                    try
                    {
                        await OnSessionTerminating(clientId, reason);
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            await OnSessionError(clientId, SessionStage.Termination, e);
                        }
                        catch { /* Diaper pattern - intentional */ }
                    }
                    RemoveSession(clientId);
                    server.Close(clientId);
                }

                /// <summary>
                ///   Gets the account data for a certain id. It will return
                ///   <code>default(AccountDataType)</code> if it is not
                ///   found, and will raise an error if something wrong goes
                ///   on while fetching. This method is asynchronous, but
                ///   nevertheless any error occuring here will cause the
                ///   session halt and the client will be kicked from the
                ///   game due to the internal error.
                /// </summary>
                /// <param name="id">The id of the account to get the data from</param>
                /// <returns>The account data</returns>
                protected abstract Task<AccountDataType> FindAccount(AccountIDType id);

                /// <summary>
                ///   Starts the session for a connection id with the given account
                ///   data. The session was already added to the mapping, but this
                ///   method is the one that initializes the actual player in the
                ///   actual game: objects, interactions, and whatever is needed.
                ///   Any error raised here will cause a session halt and the client
                ///   will be kicked from the game due to the internal error.
                /// </summary>
                /// <param name="clientId">The id of the connection whose session is being initialized</param>
                /// <param name="accountDta">The account data to initialize the session for</param>
                protected abstract Task OnSessionStarting(ulong clientId, AccountDataType accountData);

                /// <summary>
                ///   Terminates the session for a connection id with a given reason.
                ///   The connection already received a kick message, so far, but the
                ///   session still exists for now. The cleanup to do here involves
                ///   removing all the game objects and logic related to this session.
                ///   This is: removing the client from the game, effectively.
                /// </summary>
                /// <param name="clientId">The id of the connection whose session is being terminated</param>
                /// <param name="reason">The kick reason. It will be null if it was graceful logout</param>
                /// <returns></returns>
                protected abstract Task OnSessionTerminating(ulong clientId, Kicked reason);

                /// <summary>
                ///   Handles any error, typically logging it, at a given session handling
                ///   stage, and for a particular client. This will not cause the error to
                ///   be forgiven: the connection will be closed anyway. For the termination
                ///   stage, the session will still exist.
                /// </summary>
                /// <param name="clientId">The id of the connection whose session handling caused the error</param>
                /// <param name="stage">The stage where the error occurred</param>
                /// <param name="error">The error itself</param>
                protected abstract Task OnSessionError(ulong clientId, SessionStage stage, Exception error);
            }
        }
    }
}