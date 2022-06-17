using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Debug
{
    public interface IDebugVmodule
    {
        RpcRequest BuildRequest(string pattern, object id = null);
        Task<object> SendRequestAsync(string pattern, object id = null);
    }
}