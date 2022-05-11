using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Filters
{
    public interface IEthGetFilterChangesForBlockOrTransaction
    {
        RpcRequest BuildRequest(HexBigInteger filterId, object id = null);
        Task<string[]> SendRequestAsync(HexBigInteger filterId, object id = null);
    }
}