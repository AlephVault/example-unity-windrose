using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Eth
{
    public interface IEthSyncing: IGenericRpcRequestResponseHandlerNoParam<object>
    {
#if !DOTNET35
        Task<SyncingOutput> SendRequestAsync(object id = null);
#endif
    }
}