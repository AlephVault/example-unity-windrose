using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.GethEth
{
    public interface IEthPendingTransactions
    {
        RpcRequest BuildRequest(object id = null);
        Task<Transaction[]> SendRequestAsync(object id = null);
    }
}