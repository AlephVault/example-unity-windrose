using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Blocks
{
    public interface IEthGetBlockWithTransactionsHashesByHash
    {
        RpcRequest BuildRequest(string blockHash, object id = null);
        Task<BlockWithTransactionHashes> SendRequestAsync(string blockHash, object id = null);
    }
}