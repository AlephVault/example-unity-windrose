using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Debug.DTOs;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json.Linq;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Debug
{
    public interface IDebugTraceCall
    {
        RpcRequest BuildRequest(CallInput callArgs, string blockNrOrHash, TraceCallOptions options, object id = null);
        Task<JObject> SendRequestAsync(CallInput callArgs, string blockNrOrHash, TraceCallOptions options, object id = null);
    }
}