using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Filters
{
    public interface IEthNewFilter
    {
        RpcRequest BuildRequest(NewFilterInput newFilterInput, object id = null);
        Task<HexBigInteger> SendRequestAsync(NewFilterInput newFilterInput, object id = null);
    }
}