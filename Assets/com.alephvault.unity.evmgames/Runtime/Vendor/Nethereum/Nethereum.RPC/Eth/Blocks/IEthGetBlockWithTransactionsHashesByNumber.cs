using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Blocks
{
    public interface IEthGetBlockWithTransactionsHashesByNumber
    {
        RpcRequest BuildRequest(BlockParameter blockParameter, object id = null);
        RpcRequest BuildRequest(HexBigInteger number, object id = null);
        Task<BlockWithTransactionHashes> SendRequestAsync(BlockParameter blockParameter, object id = null);
        Task<BlockWithTransactionHashes> SendRequestAsync(HexBigInteger number, object id = null);
    }
}