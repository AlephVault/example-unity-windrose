using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Filters
{
    public interface IEthGetLogs
    {
        RpcRequest BuildRequest(NewFilterInput newFilter, object id = null);
        Task<FilterLog[]> SendRequestAsync(NewFilterInput newFilter, object id = null);
    }
}