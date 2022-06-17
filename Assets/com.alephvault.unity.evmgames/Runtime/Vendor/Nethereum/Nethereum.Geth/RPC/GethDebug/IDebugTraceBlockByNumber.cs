using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Debug.DTOs;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using Newtonsoft.Json.Linq;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Debug
{
    public interface IDebugTraceBlockByNumber
    {
        RpcRequest BuildRequest(ulong blockNumber, TraceTransactionOptions options, object id = null);
        Task<JArray> SendRequestAsync(ulong blockNumber, TraceTransactionOptions options = null, object id = null);
    }
}