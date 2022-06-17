using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Debug
{
    /// <Summary>
    ///     Stops an ongoing CPU profile.
    /// </Summary>
    public class DebugStopCPUProfile : GenericRpcRequestResponseHandlerNoParam<object>, IDebugStopCPUProfile
    {
        public DebugStopCPUProfile(IClient client) : base(client, ApiMethods.debug_stopCPUProfile.ToString())
        {
        }
    }
}