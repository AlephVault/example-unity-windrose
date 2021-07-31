using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AlephVault.Unity.MMO.Authoring.Behaviours.Authentication;
using AlephVault.Unity.MMO.Types;
using AlephVault.Unity.MMO.Types.Authentication;
using AlephVault.Unity.MMO.Types.Realms;


namespace AlephVault.Unity.MMO
{
    namespace Authoring
    {
        namespace Behaviours
        {
            namespace Realms
            {
                /// <summary>
                ///   This realm involves a single profile
                ///   per account. Such profile is embedded
                ///   in the same account (i.e. loading the
                ///   account involves loading the profile,
                ///   and any missing data is game-specific).
                /// </summary>
                /// <typeparam name="AccountIDType">The type of the account id (e.g. int)</typeparam>
                /// <typeparam name="AccountType">The type of the account preview data</typeparam>
                /// <typeparam name="AccountPreviewType">The type of the account data</typeparam>
                public abstract class BasicRealm<AccountIDType, AccountPreviewType, AccountType> : Realm
                    where AccountPreviewType : IAccountPreview<AccountIDType>
                    where AccountType : IAccount<AccountIDType, AccountPreviewType>
                {
                    private const string AccountDataSessionKey = "__AV:MMO__:ACCOUNT";
                    private const string InternalError = "__AV:MMO__:INTERNAL-ERROR";
                    private const string MissingAccount = "__AV:MMO__:MISSING-ACCOUNT";

                    /// <summary>
                    ///   Loads an account data by its ID.
                    /// </summary>
                    /// <param name="id">The ID of the account to load</param>
                    /// <returns>The account data</returns>
                    protected abstract Task<AccountType> LoadAccount(AccountIDType id);

                    /// <summary>
                    ///   Attends a particular login failure.
                    ///   It may also use side-effects to register the failure.
                    /// </summary>
                    /// <param name="id">The ID of the account to load</param>
                    /// <returns>The account data</returns>
                    protected abstract Task AttendLoginFailure(ulong clientId, Response response, Authenticator.AccountId accountId);

                    /// <summary>
                    ///   Initializes the account in the game itself.
                    ///   It may also use side-effects to clear former failures.
                    /// </summary>
                    /// <param name="clientId">The connection this account is being initialized for</param>
                    /// <param name="account">The account being initialized</param>
                    protected abstract Task InitializeAccount(ulong clientId, AccountType account);

                    /// <summary>
                    ///   Cleans the account up in the game itself.
                    /// </summary>
                    /// <param name="clientId">The connection this account is being initialized for</param>
                    /// <param name="account">The account being initialized</param>
                    protected abstract Task ClearAccount(ulong clientId, AccountType account);

                    /// <summary>
                    ///   Sets the sessin's account to a retrieved account
                    ///   instance/data.
                    /// </summary>
                    /// <param name="clientId">The connection ID to set the account in its session</param>
                    /// <param name="account">The whole account data</param>
                    protected void SetCurrentAccount(ulong clientId, AccountType account)
                    {
                        try
                        {
                            Authenticator.SetSessionData(clientId, AccountDataSessionKey, account);
                        }
                        catch(System.Exception)
                        {
                            throw new LoginAborted(InternalError, "Internal error");
                        }
                    }

                    protected override async Task OnAccountLoggedOut(ulong clientId, Reason reason, Authenticator.AccountId accountId)
                    {
                        // This whole logic only applies to this realm.
                        if (accountId.Item2 == Name())
                        {
                            await ClearAccount(clientId, (AccountType)Authenticator.GetSessionData(clientId, AccountDataSessionKey));
                            Authenticator.RemoveSessionData(clientId, AccountDataSessionKey);
                        }
                    }

                    protected override async Task OnAccountLoginFailed(ulong clientId, Response response, Authenticator.AccountId accountId)
                    {
                        // This whole logic only applies to this realm.
                        if (accountId.Item2 == Name())
                        {
                            await AttendLoginFailure(clientId, response, accountId);
                        }
                    }

                    protected override async Task OnAccountLoginOK(ulong clientId, Response response, Authenticator.AccountId accountId)
                    {
                        // This whole logic only applies to this realm.
                        if (accountId.Item2 == Name())
                        {
                            if (accountId.Item1 is AccountIDType)
                            {
                                AccountType account = await LoadAccount((AccountIDType)accountId.Item1);
                                if (account.Equals(default(AccountIDType)))
                                {
                                    SetCurrentAccount(clientId, account);
                                    // If the following line throws any exception, it will kick
                                    // the account with an unknown error.
                                    await InitializeAccount(clientId, account);
                                }
                                else
                                {
                                    throw new LoginAborted(MissingAccount, "Missing account");
                                }
                            }
                            else
                            {
                                throw new System.Exception("Invalid account id type");
                            }
                        }
                    }
                }
            }
        }
    }
}
