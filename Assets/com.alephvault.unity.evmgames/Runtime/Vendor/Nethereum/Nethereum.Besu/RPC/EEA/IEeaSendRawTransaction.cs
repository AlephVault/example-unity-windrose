using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.Besu.RPC.EEA
{
    public interface IEeaSendRawTransaction
    {
        Task<string> SendRequestAsync(string signedTransaction, object id = null);
        RpcRequest BuildRequest(string signedTransaction, object id = null);
    }
}