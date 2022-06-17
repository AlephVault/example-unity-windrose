using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Debug
{
    public interface IDebugGoTrace
    {
        RpcRequest BuildRequest(string fileName, int seconds, object id = null);
        Task<object> SendRequestAsync(string fileName, int seconds, object id = null);
    }
}