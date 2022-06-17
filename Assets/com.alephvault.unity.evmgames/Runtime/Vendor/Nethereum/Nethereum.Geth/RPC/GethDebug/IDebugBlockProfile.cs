using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Debug
{
    public interface IDebugBlockProfile
    {
        RpcRequest BuildRequest(string file, long seconds, object id = null);
        Task<object> SendRequestAsync(string file, long seconds, object id = null);
    }
}