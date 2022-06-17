using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Debug
{
    public interface IDebugSetBlockProfileRate
    {
        RpcRequest BuildRequest(long rate, object id = null);
        Task<object> SendRequestAsync(long rate, object id = null);
    }
}