using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.Quorum.RPC
{
    public class QuorumCanonicalHash : RpcRequestResponseHandler<string>, IQuorumCanonicalHash
    {
        public QuorumCanonicalHash(IClient client) : base(client, ApiMethods.quorum_canonicalHash.ToString())
        {
        }

        public Task<string> SendRequestAsync(long blockNumber, object id = null)
        {
            return base.SendRequestAsync(id, blockNumber);
        }

        public RpcRequest BuildRequest(long blockNumber, object id = null)
        {
            return base.BuildRequest(id, blockNumber);
        }
    }
}