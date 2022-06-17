using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure;

namespace AlephVault.Unity.EVMGames.Nethereum.Besu.RPC.Permissioning
{
    /// <Summary>
    ///     Adds nodes to the nodes whitelist.
    /// </Summary>
    public class PermGetNodesWhitelist : GenericRpcRequestResponseHandlerNoParam<string[]>, IPermGetNodesWhitelist
    {
        public PermGetNodesWhitelist(IClient client) : base(client, ApiMethods.perm_getNodesWhitelist.ToString())
        {
        }
    }
}