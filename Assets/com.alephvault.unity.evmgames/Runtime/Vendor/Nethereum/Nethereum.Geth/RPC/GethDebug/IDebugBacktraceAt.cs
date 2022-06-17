using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Debug
{
    public interface IDebugBacktraceAt
    {
        RpcRequest BuildRequest(string fileAndLine, object id = null);
        Task<string> SendRequestAsync(string fileAndLine, object id = null);
    }
}