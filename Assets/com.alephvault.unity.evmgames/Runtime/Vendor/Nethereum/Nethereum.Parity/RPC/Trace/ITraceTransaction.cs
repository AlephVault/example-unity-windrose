using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using Newtonsoft.Json.Linq;

namespace AlephVault.Unity.EVMGames.Nethereum.Parity.RPC.Trace
{
    public interface ITraceTransaction
    {
        RpcRequest BuildRequest(string transactionHash, object id = null);
        Task<JArray> SendRequestAsync(string transactionHash, object id = null);
    }
}
