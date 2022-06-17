using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.Besu.RPC.Clique
{
    public interface ICliquePropose
    {
        Task<bool> SendRequestAsync(string address, bool addSigner, object id = null);
        RpcRequest BuildRequest(string address, bool addSigner, object id = null);
    }
}