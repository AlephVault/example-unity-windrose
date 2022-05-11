using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Filters
{
    public interface IEthGetFilterChangesForEthNewFilter
    {
        RpcRequest BuildRequest(HexBigInteger filterId, object id = null);
        Task<FilterLog[]> SendRequestAsync(HexBigInteger filterId, object id = null);
    }
}