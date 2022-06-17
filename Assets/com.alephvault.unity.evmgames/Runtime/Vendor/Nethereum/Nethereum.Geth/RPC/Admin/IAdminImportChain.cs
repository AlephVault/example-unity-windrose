using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Admin
{
    public interface IAdminImportChain
    {
        RpcRequest BuildRequest(string filePath, object id = null);
        Task<bool> SendRequestAsync(string filePath, object id = null);
    }
}