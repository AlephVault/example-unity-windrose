using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.Parity.RPC.Trace.TraceDTOs;
using Newtonsoft.Json.Linq;

namespace AlephVault.Unity.EVMGames.Nethereum.Parity.RPC.Trace
{
    public interface ITraceFilter
    {
        RpcRequest BuildRequest(TraceFilterDTO traceFilter, object id = null);
        Task<JArray> SendRequestAsync(TraceFilterDTO traceFilter, object id = null);
    }
}