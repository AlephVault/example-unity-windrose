using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Debug.DTOs;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using Newtonsoft.Json.Linq;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Debug
{
    public interface IDebugTraceBlockFromFile
    {
        RpcRequest BuildRequest(string filePath, TraceTransactionOptions options, object id = null);
        Task<JArray> SendRequestAsync(string filePath, TraceTransactionOptions options, object id = null);
    }
}