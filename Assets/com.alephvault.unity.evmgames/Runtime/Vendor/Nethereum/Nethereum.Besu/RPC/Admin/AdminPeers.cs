using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure;
using Newtonsoft.Json.Linq;

namespace AlephVault.Unity.EVMGames.Nethereum.Besu.RPC.Admin
{
    /// <Summary>
    ///     Returns networking information about connected remote nodes.
    /// </Summary>
    public class AdminPeers : GenericRpcRequestResponseHandlerNoParam<JArray>, IAdminPeers
    {
        public AdminPeers(IClient client) : base(client, ApiMethods.admin_peers.ToString())
        {
        }
    }
}