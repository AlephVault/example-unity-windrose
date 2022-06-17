using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.Quorum.RPC
{
    public interface IQuorumVote
    {
        RpcRequest BuildRequest(string hash, object id = null);
        Task<string> SendRequestAsync(string hash, object id = null);
    }
}