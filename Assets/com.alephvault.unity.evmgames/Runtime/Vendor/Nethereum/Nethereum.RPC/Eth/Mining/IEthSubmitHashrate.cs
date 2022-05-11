using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Mining
{
    public interface IEthSubmitHashrate
    {
        RpcRequest BuildRequest(string hashRate, string clientId, object id = null);
        Task<bool> SendRequestAsync(string hashRate, string clientId, object id = null);
    }
}