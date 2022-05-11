using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Shh.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Shh.MessageFilter
{
    public interface IShhGetFilterMessages : IGenericRpcRequestResponseHandlerParamString<ShhMessage[]>
    {

    }
}