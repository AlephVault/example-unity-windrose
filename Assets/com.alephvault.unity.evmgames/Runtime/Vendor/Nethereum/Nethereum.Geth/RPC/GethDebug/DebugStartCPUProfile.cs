using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Debug
{
    /// <Summary>
    ///     Turns on CPU profiling indefinitely, writing to the given file.
    /// </Summary>
    public class DebugStartCPUProfile : RpcRequestResponseHandler<object>, IDebugStartCPUProfile
    {
        public DebugStartCPUProfile(IClient client) : base(client, ApiMethods.debug_startCPUProfile.ToString())
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