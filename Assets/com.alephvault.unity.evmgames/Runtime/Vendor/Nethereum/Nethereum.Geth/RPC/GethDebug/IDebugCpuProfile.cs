using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Debug
{
    public interface IDebugCpuProfile
    {
        RpcRequest BuildRequest(string filePath, int seconds, object id = null);
        Task<object> SendRequestAsync(string filePath, int seconds, object id = null);
    }
}