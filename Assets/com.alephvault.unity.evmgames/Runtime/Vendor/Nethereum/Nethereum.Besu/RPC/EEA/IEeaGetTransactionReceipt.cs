using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.Besu.RPC.EEA.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.Besu.RPC.EEA
{
    public interface IEeaGetTransactionReceipt
    {
        Task<EeaTransactionReceipt> SendRequestAsync(string transactionHash, object id = null);
        RpcRequest BuildRequest(string transactionHash, object id = null);
    }
}