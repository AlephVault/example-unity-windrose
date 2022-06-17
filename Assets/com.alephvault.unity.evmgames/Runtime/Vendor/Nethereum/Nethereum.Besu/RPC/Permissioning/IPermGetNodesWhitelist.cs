using AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure;

namespace AlephVault.Unity.EVMGames.Nethereum.Besu.RPC.Permissioning
{
    public interface IPermGetNodesWhitelist : IGenericRpcRequestResponseHandlerNoParam<string[]>
    {
    }
}