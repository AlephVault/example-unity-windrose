using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.Rsk.RPC.RskEth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.Rsk.RPC.RskEth
{
    public interface IRskEthGetBlockWithTransactionsByHash
    {
        Task<RskBlockWithTransactions> SendRequestAsync(string blockHash, object id = null);
        RpcRequest BuildRequest(string blockHash, object id = null);
    }
}