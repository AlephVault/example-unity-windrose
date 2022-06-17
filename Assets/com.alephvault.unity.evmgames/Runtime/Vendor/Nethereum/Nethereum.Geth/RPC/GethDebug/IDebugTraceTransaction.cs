using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Debug.DTOs;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using Newtonsoft.Json.Linq;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Debug
{
    public interface IDebugTraceTransaction
    {
        RpcRequest BuildRequest(string txnHash, TraceTransactionOptions options, object id = null);
        Task<JObject> SendRequestAsync(string txnHash, TraceTransactionOptions options, object id = null);
    }
}