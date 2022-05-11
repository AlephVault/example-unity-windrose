using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Shh.MessageFilter
{
    public class ShhDeleteMessageFilter : GenericRpcRequestResponseHandlerParamString<bool>, IShhDeleteMessageFilter
    {
        public ShhDeleteMessageFilter(IClient client) : base(client, ApiMethods.shh_deleteMessageFilter.ToString())
        {
        }
    }
}
