using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure;

namespace AlephVault.Unity.EVMGames.Nethereum.Quorum.RPC
{
    public class QuorumPauseBlockMaker : GenericRpcRequestResponseHandlerNoParam<object>, IQuorumPauseBlockMaker
    {
        public QuorumPauseBlockMaker(IClient client) : base(client, ApiMethods.quorum_pauseBlockMaker.ToString())
        {
        }
    }

    public interface IQuorumPauseBlockMaker : IGenericRpcRequestResponseHandlerNoParam<object>
    {

    }
}