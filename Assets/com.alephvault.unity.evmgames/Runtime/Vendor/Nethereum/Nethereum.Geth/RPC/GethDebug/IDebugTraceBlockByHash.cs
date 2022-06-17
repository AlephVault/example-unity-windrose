using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Debug.DTOs;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using Newtonsoft.Json.Linq;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Debug
{
    public interface IDebugTraceBlockByHash
    {
        RpcRequest BuildRequest(string hash, TraceTransactionOptions options, object id = null);
        Task<JArray> SendRequestAsync(string hash,TraceTransactionOptions options, object id = null);
    }
}