using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Uncles
{
    public interface IEthGetUncleByBlockNumberAndIndex
    {
        RpcRequest BuildRequest(BlockParameter blockParameter, HexBigInteger uncleIndex, object id = null);
        Task<BlockWithTransactionHashes> SendRequestAsync(BlockParameter blockParameter, HexBigInteger uncleIndex, object id = null);
    }
}