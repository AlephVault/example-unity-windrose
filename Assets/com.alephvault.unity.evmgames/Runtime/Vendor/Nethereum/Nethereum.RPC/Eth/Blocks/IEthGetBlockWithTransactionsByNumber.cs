using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Blocks
{
    public interface IEthGetBlockWithTransactionsByNumber
    {
        RpcRequest BuildRequest(BlockParameter blockParameter, object id = null);
        RpcRequest BuildRequest(HexBigInteger number, object id = null);
        Task<BlockWithTransactions> SendRequestAsync(BlockParameter blockParameter, object id = null);
        Task<BlockWithTransactions> SendRequestAsync(HexBigInteger number, object id = null);
    }
}