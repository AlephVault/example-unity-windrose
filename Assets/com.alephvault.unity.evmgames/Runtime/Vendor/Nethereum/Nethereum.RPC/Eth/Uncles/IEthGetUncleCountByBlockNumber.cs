using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Uncles
{
    public interface IEthGetUncleCountByBlockNumber
    {
        RpcRequest BuildRequest(HexBigInteger blockNumber, object id = null);
        Task<HexBigInteger> SendRequestAsync(HexBigInteger blockNumber, object id = null);
    }
}