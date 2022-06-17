using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.Besu.RPC.Clique
{
    public interface ICliqueGetSignersAtHash
    {
        Task<string[]> SendRequestAsync(string blockHash, object id = null);
        RpcRequest BuildRequest(string blockHash, object id = null);
    }

    /// <Summary>
    ///     Lists signers for the specified block.
    /// </Summary>
    public class CliqueGetSignersAtHash : RpcRequestResponseHandler<string[]>, ICliqueGetSignersAtHash
    {
        public CliqueGetSignersAtHash(IClient client) : base(client, ApiMethods.clique_getSignersAtHash.ToString())
        {
        }

        public Task<string[]> SendRequestAsync(string blockHash, object id = null)
        {
            return base.SendRequestAsync(id, blockHash);
        }

        public RpcRequest BuildRequest(string blockHash, object id = null)
        {
            return base.BuildRequest(id, blockHash);
        }
    }
}