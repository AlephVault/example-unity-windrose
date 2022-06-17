using System;
using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Admin
{
    /// <Summary>
    /// ImportChain imports a blockchain from a local file.
    /// </Summary>
    public class AdminImportChain : RpcRequestResponseHandler<bool>, IAdminImportChain
    {
        public AdminImportChain(IClient client) : base(client, ApiMethods.admin_importChain.ToString())
        {
        }

        public RpcRequest BuildRequest(string filePath, object id = null)
        {
            if (filePath == null) throw new ArgumentNullException(nameof(filePath));
            return base.BuildRequest(id, filePath);
        }

        public Task<bool> SendRequestAsync(string filePath, object id = null)
        {
            if (filePath == null) throw new ArgumentNullException(nameof(filePath));
            return base.SendRequestAsync(id, filePath);
        }
    }
}