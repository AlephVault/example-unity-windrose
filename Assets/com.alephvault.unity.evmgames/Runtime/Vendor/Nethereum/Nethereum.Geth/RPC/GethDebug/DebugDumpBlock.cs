using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using Newtonsoft.Json.Linq;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Debug
{
    /// <Summary>
    ///     Retrieves the state that corresponds to the block number and returns a list of accounts (including storage and
    ///     code).
    /// </Summary>
    public class DebugDumpBlock : RpcRequestResponseHandler<JObject>, IDebugDumpBlock
    {
        public DebugDumpBlock(IClient client) : base(client, ApiMethods.debug_dumpBlock.ToString())
        {
        }

        public RpcRequest BuildRequest(ulong blockNumber, object id = null)
        {
            return base.BuildRequest(id, blockNumber);
        }

        public Task<JObject> SendRequestAsync(ulong blockNumber, object id = null)
        {
            return base.SendRequestAsync(id, blockNumber);
        }
    }
}