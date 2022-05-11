using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Personal
{
    /// <Summary>
    ///     personal_listAccounts
    ///     List all accounts
    ///     Parameters
    ///     none
    ///     Return
    ///     array collection with accounts
    ///     Example
    ///     personal.listAccounts
    /// </Summary>
    public class PersonalListAccounts : GenericRpcRequestResponseHandlerNoParam<string[]>, IPersonalListAccounts
    {
        public PersonalListAccounts(IClient client) : base(client, ApiMethods.personal_listAccounts.ToString())
        {
        }
    }
}