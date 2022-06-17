using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.Besu.RPC.IBFT
{
    /// <Summary>
    ///     Lists the validators defined in the specified block.
    /// </Summary>
    public class IbftGetValidatorsByBlockHash : RpcRequestResponseHandler<string[]>, IIbftGetValidatorsByBlockHash
    {
        public IbftGetValidatorsByBlockHash(IClient client) : base(client,
            ApiMethods.ibft_getValidatorsByBlockHash.ToString())
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