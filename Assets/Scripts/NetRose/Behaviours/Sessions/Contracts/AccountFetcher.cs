using System.Threading.Tasks;

namespace NetRose
{
    namespace Behaviours
    {
        namespace Sessions
        {
            namespace Contracts
            {
                /// <summary>
                ///   Account fetchers have a single method to load
                ///     the account's data in an asynchronous way.
                /// </summary>
                /// <typeparam name="AccountID">The type of the id of an account</typeparam>
                /// <typeparam name="AccountData">The type of the account data being retrieved</typeparam>
                public interface AccountFetcher<AccountID, AccountData>
                {
                    /// <summary>
                    ///   Asynchronously loads account data by its id.
                    ///     Returns <c>default(AccountData)</c> if
                    ///     absent, and can freely throw any exception
                    ///     on error.
                    /// </summary>
                    /// <param name="accountId">The id of the account to load the data</param>
                    /// <returns>The account's data</returns>
                    Task<AccountData> GetAccountData(AccountID accountId);
                }
            }
        }
    }
}
