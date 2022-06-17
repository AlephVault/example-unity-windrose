using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Debug
{
    public interface IDebugVerbosity
    {
        RpcRequest BuildRequest(int level, object id = null);
        Task<object> SendRequestAsync(int level, object id = null);
    }
}