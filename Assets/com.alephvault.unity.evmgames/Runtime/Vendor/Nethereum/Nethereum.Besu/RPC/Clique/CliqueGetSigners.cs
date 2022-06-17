using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.Besu.RPC.Clique
{
    public interface ICliqueGetSigners
    {
        Task<string[]> SendRequestAsync(BlockParameter blockParameter, object id = null);
        RpcRequest BuildRequest(BlockParameter blockParameter, object id = null);
    }

    /// <Summary>
    ///     Lists signers for the specified block.
    /// </Summary>
    public class CliqueGetSigners : RpcRequestResponseHandler<string[]>, ICliqueGetSigners
    {
        public CliqueGetSigners(IClient client) : base(client, ApiMethods.clique_getSigners.ToString())
        {
        }

        public Task<string[]> SendRequestAsync(BlockParameter blockParameter, object id = null)
        {
            return base.SendRequestAsync(id, blockParameter);
        }

        public RpcRequest BuildRequest(BlockParameter blockParameter, object id = null)
        {
            return base.BuildRequest(id, blockParameter);
        }
    }
}