using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Debug.DTOs;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using Newtonsoft.Json.Linq;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Debug
{
    /// <Summary>
    ///     Similar to debug_traceBlock, traceBlockByNumber accepts a block number and will replay the block that is already
    ///     present in the database.
    /// </Summary>
    public class DebugTraceBlockByNumber : RpcRequestResponseHandler<JArray>, IDebugTraceBlockByNumber
    {
        public DebugTraceBlockByNumber(IClient client) : base(client, ApiMethods.debug_traceBlockByNumber.ToString())
        {
        }

        public RpcRequest BuildRequest(ulong blockNumber, TraceTransactionOptions options, object id = null)
        {
            return base.BuildRequest(id, new HexBigInteger(blockNumber), options);
        }

        public Task<JArray> SendRequestAsync(ulong blockNumber, TraceTransactionOptions options, object id = null)
        {
            return base.SendRequestAsync(id, new HexBigInteger(blockNumber), options);
        }
    }
}