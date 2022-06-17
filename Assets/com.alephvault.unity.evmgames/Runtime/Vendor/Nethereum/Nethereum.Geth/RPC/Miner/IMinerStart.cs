using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Miner
{
    public interface IMinerStart
    {
        RpcRequest BuildRequest(int number, object id = null);
        Task<bool> SendRequestAsync(object id = null);
        Task<bool> SendRequestAsync(int number, object id = null);
    }
}