using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json.Linq;

namespace AlephVault.Unity.EVMGames.Nethereum.Parity.RPC.Trace
{
    public interface ITraceCall
    {
        RpcRequest BuildRequest(CallInput callInput, TraceType[] typeOfTrace, BlockParameter block, object id = null);
        Task<JObject> SendRequestAsync(CallInput callInput, TraceType[] typeOfTrace, BlockParameter block, object id = null);
    }
}