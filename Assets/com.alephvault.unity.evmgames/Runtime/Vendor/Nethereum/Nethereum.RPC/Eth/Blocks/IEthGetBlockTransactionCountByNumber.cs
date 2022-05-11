using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Blocks
{
    public interface IEthGetBlockTransactionCountByNumber
    {
        RpcRequest BuildRequest(BlockParameter block, object id = null);
        Task<HexBigInteger> SendRequestAsync(object id = null);
        Task<HexBigInteger> SendRequestAsync(BlockParameter block, object id = null);
    }
}