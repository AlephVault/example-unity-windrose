using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using Newtonsoft.Json.Linq;

namespace AlephVault.Unity.EVMGames.Nethereum.Parity.RPC.Trace
{
    public interface ITraceRawTransaction
    {
        RpcRequest BuildRequest(string rawTransaction, TraceType[] traceTypes, object id = null);
        Task<JObject> SendRequestAsync(string rawTransaction, TraceType[] traceTypes, object id = null);
    }
}