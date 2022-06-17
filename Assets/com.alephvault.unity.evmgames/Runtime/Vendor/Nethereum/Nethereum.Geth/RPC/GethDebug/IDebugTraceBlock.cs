using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Debug.DTOs;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using Newtonsoft.Json.Linq;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Debug
{
    public interface IDebugTraceBlock
    {
        RpcRequest BuildRequest(string blockRlpHex, TraceTransactionOptions options, object id = null);
        Task<JArray> SendRequestAsync(string blockRlpHex, TraceTransactionOptions options, object id = null);
    }
}