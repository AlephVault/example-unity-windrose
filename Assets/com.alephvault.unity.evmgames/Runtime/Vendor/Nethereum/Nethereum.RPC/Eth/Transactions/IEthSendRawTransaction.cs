using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Transactions
{
    public interface IEthSendRawTransaction
    {
        RpcRequest BuildRequest(string signedTransactionData, object id = null);
        Task<string> SendRequestAsync(string signedTransactionData, object id = null);
    }
}