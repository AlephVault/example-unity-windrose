using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using Newtonsoft.Json.Linq;

namespace AlephVault.Unity.EVMGames.Nethereum.Besu.RPC.Debug
{
    public interface IDebugTraceTransaction
    {
        Task<JObject> SendRequestAsync(string transactionHash, object id = null);
        RpcRequest BuildRequest(string transactionHash, object id = null);
    }
}