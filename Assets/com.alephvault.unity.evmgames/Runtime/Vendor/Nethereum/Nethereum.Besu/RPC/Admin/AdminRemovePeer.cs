using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.Besu.RPC.Admin
{
    /// <Summary>
    ///     Removes a static node.
    /// </Summary>
    public class AdminRemovePeer : RpcRequestResponseHandler<bool>, IAdminRemovePeer
    {
        public AdminRemovePeer(IClient client) : base(client, ApiMethods.admin_removePeer.ToString())
        {
        }

        public Task<bool> SendRequestAsync(string enodeUrl, object id = null)
        {
            return base.SendRequestAsync(id, enodeUrl);
        }

        public RpcRequest BuildRequest(string enodeUrl, object id = null)
        {
            return base.BuildRequest(id, enodeUrl);
        }
    }
}