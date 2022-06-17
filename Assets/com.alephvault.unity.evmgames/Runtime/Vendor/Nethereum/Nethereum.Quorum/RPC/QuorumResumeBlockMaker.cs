using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure;

namespace AlephVault.Unity.EVMGames.Nethereum.Quorum.RPC
{
    public class QuorumResumeBlockMaker : GenericRpcRequestResponseHandlerNoParam<object>, IQuorumResumeBlockMaker
    {
        public QuorumResumeBlockMaker(IClient client) : base(client, ApiMethods.quorum_resumeBlockMaker.ToString())
        {
        }
    }

    public interface IQuorumResumeBlockMaker : IGenericRpcRequestResponseHandlerNoParam<object>
    {

    }
}