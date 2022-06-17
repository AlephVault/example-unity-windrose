using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.Quorum.RPC
{
    public interface IQuorumIsVoter
    {
        RpcRequest BuildRequest(string address, object id = null);
        Task<bool> SendRequestAsync(string address, object id = null);
    }
}