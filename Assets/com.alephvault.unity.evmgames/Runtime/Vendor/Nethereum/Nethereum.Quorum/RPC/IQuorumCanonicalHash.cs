using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.Quorum.RPC
{
    public interface IQuorumCanonicalHash
    {
        RpcRequest BuildRequest(long blockNumber, object id = null);
        Task<string> SendRequestAsync(long blockNumber, object id = null);
    }
}