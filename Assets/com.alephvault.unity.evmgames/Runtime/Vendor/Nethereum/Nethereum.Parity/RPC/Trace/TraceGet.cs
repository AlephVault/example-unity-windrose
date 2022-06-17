using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using Newtonsoft.Json.Linq;

namespace AlephVault.Unity.EVMGames.Nethereum.Parity.RPC.Trace
{
    /// <Summary>
    ///     Returns trace at given position.
    /// </Summary>
    public class TraceGet : RpcRequestResponseHandler<JObject>, ITraceGet
    {
        public TraceGet(IClient client) : base(client, ApiMethods.trace_get.ToString())
        {
        }

        public Task<JObject> SendRequestAsync(string transactionHash, HexBigInteger[] index, object id = null)
        {
            return base.SendRequestAsync(id, transactionHash, index);
        }

        public RpcRequest BuildRequest(string transactionHash, HexBigInteger[] index, object id = null)
        {
            return base.BuildRequest(id, transactionHash, index);
        }
    }
}