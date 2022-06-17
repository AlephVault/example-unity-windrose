using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.Besu.RPC.Clique
{
    public interface ICliqueDiscard
    {
        Task<bool> SendRequestAsync(string addressSigner, object id = null);
        RpcRequest BuildRequest(string addressSigner, object id = null);
    }
}