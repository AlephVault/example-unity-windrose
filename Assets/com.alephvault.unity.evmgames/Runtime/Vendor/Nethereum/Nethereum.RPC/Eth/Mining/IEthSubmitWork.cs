using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Mining
{
    public interface IEthSubmitWork
    {
        RpcRequest BuildRequest(string nonce, string header, string mix, object id = null);
        Task<bool> SendRequestAsync(string nonce, string header, string mix, object id = null);
    }
}