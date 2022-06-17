using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Debug
{
    /// <Summary>
    ///     Returns a printed representation of the stacks of all goroutines.
    /// </Summary>
    public class DebugStacks : GenericRpcRequestResponseHandlerNoParam<string>, IDebugStacks
    {
        public DebugStacks(IClient client) : base(client, ApiMethods.debug_stacks.ToString())
        {
        }
    }
}