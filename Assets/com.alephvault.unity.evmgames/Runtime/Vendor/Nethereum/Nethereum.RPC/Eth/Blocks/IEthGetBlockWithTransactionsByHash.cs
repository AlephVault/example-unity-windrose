using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Blocks
{
    public interface IEthGetBlockWithTransactionsByHash
    {
        RpcRequest BuildRequest(string blockHash, object id = null);
        Task<BlockWithTransactions> SendRequestAsync(string blockHash, object id = null);
    }
}