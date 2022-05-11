using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Transactions
{
    public interface IEthGetTransactionReceipt
    {
        RpcRequest BuildRequest(string transactionHash, object id = null);
        Task<TransactionReceipt> SendRequestAsync(string transactionHash, object id = null);
    }
}