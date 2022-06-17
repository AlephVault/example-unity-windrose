using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure;

namespace AlephVault.Unity.EVMGames.Nethereum.Besu.RPC.Permissioning
{
    /// <Summary>
    ///     Lists accounts (participants) in the accounts whitelist.
    /// </Summary>
    public class PermGetAccountsWhitelist : GenericRpcRequestResponseHandlerNoParam<string[]>, IPermGetAccountsWhitelist
    {
        public PermGetAccountsWhitelist(IClient client) : base(client, ApiMethods.perm_getAccountsWhitelist.ToString())
        {
        }
    }
}