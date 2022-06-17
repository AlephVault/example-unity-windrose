using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Admin
{
    public interface IAdminRemovePeer
    {
        RpcRequest BuildRequest(string enodeUrl, object id = null);
        Task<bool> SendRequestAsync(string enodeUrl, object id = null);
    }
}