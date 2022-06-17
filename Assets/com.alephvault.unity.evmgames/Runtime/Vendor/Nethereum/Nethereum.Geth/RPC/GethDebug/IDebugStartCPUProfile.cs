using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Debug
{
    public interface IDebugStartCPUProfile
    {
        RpcRequest BuildRequest(string filePath, object id = null);
        Task<object> SendRequestAsync(string filePath, object id = null);
    }
}