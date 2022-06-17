using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.Besu.RPC.Permissioning
{
    public interface IPermRemoveAccountsFromWhitelist
    {
        Task<string> SendRequestAsync(string[] addresses, object id = null);
        RpcRequest BuildRequest(string[] addresses, object id = null);
    }
}