using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Debug
{
    public interface IDebugSeedHash
    {
        RpcRequest BuildRequest(ulong blockNumber, object id = null);
        Task<string> SendRequestAsync(ulong blockNumber, object id = null);
    }
}