using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure;

namespace AlephVault.Unity.EVMGames.Nethereum.Quorum.RPC
{
    public class QuorumMakeBlock : GenericRpcRequestResponseHandlerNoParam<string>, IQuorumMakeBlock
    {
        public QuorumMakeBlock(IClient client) : base(client, ApiMethods.quorum_makeBlock.ToString())
        {
        }
    }

    public interface IQuorumMakeBlock : IGenericRpcRequestResponseHandlerNoParam<string>
    {

    }
}