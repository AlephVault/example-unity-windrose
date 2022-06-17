using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Debug
{
    /// <Summary>
    ///     Starts writing a Go runtime trace to the given file.
    /// </Summary>
    public class DebugStartGoTrace : RpcRequestResponseHandler<object>, IDebugStartGoTrace
    {
        public DebugStartGoTrace(IClient client) : base(client, ApiMethods.debug_startGoTrace.ToString())
        {
        }

        public RpcRequest BuildRequest(string filePath, object id = null)
        {
            return base.BuildRequest(id, filePath);
        }

        public Task<object> SendRequestAsync(string filePath, object id = null)
        {
            return base.SendRequestAsync(id, filePath);
        }
    }
}