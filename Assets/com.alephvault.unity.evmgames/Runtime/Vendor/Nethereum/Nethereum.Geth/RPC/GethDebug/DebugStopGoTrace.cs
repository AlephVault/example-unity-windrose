using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Debug
{
    /// <Summary>
    ///     Stops writing the Go runtime trace.
    /// </Summary>
    public class DebugStopGoTrace : GenericRpcRequestResponseHandlerNoParam<object>, IDebugStopGoTrace
    {
        public DebugStopGoTrace(IClient client) : base(client, ApiMethods.debug_stopGoTrace.ToString())
        {
        }
    }
}