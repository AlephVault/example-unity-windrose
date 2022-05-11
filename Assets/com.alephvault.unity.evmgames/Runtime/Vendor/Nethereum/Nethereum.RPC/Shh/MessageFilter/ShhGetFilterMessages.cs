using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Shh.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Shh.MessageFilter
{
    public class ShhGetFilterMessages : GenericRpcRequestResponseHandlerParamString<ShhMessage[]>, IShhGetFilterMessages
    {
        public ShhGetFilterMessages(IClient client) : base(client, ApiMethods.shh_getFilterMessages.ToString())
        {
        }
    }
}