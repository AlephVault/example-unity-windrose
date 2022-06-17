using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.Besu.RPC.Permissioning
{
    /// <Summary>
    ///     Removes nodes from the nodes whitelist.
    /// </Summary>
    public class PermRemoveNodesFromWhitelist : RpcRequestResponseHandler<string>, IPermRemoveNodesFromWhitelist
    {
        public PermRemoveNodesFromWhitelist(IClient client) : base(client,
            ApiMethods.perm_removeNodesFromWhitelist.ToString())
        {
        }

        public Task<string> SendRequestAsync(string[] addresses, object id = null)
        {
            return base.SendRequestAsync(id, new object[] {addresses});
        }

        public RpcRequest BuildRequest(string[] addresses, object id = null)
        {
            return base.BuildRequest(id, new object[] {addresses});
        }
    }
}