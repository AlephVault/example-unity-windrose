using System;
using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.Besu.RPC.Admin
{
    /// <Summary>
    ///     Adds a static node.
    /// </Summary>
    public class AdminAddPeer : RpcRequestResponseHandler<bool>, IAdminAddPeer
    {
        public AdminAddPeer(IClient client) : base(client, ApiMethods.admin_addPeer.ToString())
        {
        }

        public RpcRequest BuildRequest(string enodeUrl, object id = null)
        {
            if (enodeUrl == null) throw new ArgumentNullException(nameof(enodeUrl));
            return base.BuildRequest(id, enodeUrl);
        }

        public Task<bool> SendRequestAsync(string enodeUrl, object id = null)
        {
            if (enodeUrl == null) throw new ArgumentNullException(nameof(enodeUrl));
            return base.SendRequestAsync(id, enodeUrl);
        }
    }
}