using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Transactions
{
    public interface IEthGetTransactionByHash
    {
        RpcRequest BuildRequest(string hashTransaction, object id = null);
        Task<Transaction> SendRequestAsync(string hashTransaction, object id = null);
    }
}