using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using Newtonsoft.Json.Linq;

namespace AlephVault.Unity.EVMGames.Nethereum.Parity.RPC.Trace
{
    /// <Summary>
    ///     Traces a call to eth_sendRawTransaction without making the call, returning the traces
    /// </Summary>
    public class TraceRawTransaction : RpcRequestResponseHandler<JObject>, ITraceRawTransaction
    {
        public TraceRawTransaction(IClient client) : base(client, ApiMethods.trace_rawTransaction.ToString())
        {
        }

        public Task<JObject> SendRequestAsync(string rawTransaction, TraceType[] traceTypes, object id = null)
        {
            return base.SendRequestAsync(id, rawTransaction, traceTypes.ConvertToStringArray());
        }

        public RpcRequest BuildRequest(string rawTransaction, TraceType[] traceTypes, object id = null)
        {
            return base.BuildRequest(id, rawTransaction, traceTypes.ConvertToStringArray());
        }
    }
}