using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Personal
{
    public interface IPersonalSignAndSendTransaction
    {
        RpcRequest BuildRequest(TransactionInput txn, string password, object id = null);
        Task<string> SendRequestAsync(TransactionInput txn, string password, object id = null);
    }
}