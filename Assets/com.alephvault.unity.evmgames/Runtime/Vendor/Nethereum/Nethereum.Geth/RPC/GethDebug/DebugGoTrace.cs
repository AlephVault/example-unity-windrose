using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Debug
{
    /// <Summary>
    ///     Turns on Go runtime tracing for the given duration and writes trace data to disk.
    /// </Summary>
    public class DebugGoTrace : RpcRequestResponseHandler<object>, IDebugGoTrace
    {
        public DebugGoTrace(IClient client) : base(client, ApiMethods.debug_goTrace.ToString())
        {
        }

        public RpcRequest BuildRequest(string fileName, int seconds, object id = null)
        {
            return base.BuildRequest(id, fileName, seconds);
        }

        public Task<object> SendRequestAsync(string fileName, int seconds, object id = null)
        {
            return base.SendRequestAsync(id, fileName, seconds);
        }
    }
}