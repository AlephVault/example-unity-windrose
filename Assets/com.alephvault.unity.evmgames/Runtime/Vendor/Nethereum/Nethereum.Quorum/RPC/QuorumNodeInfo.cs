using System;
using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.Quorum.RPC.DTOs;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure;

namespace AlephVault.Unity.EVMGames.Nethereum.Quorum.RPC
{
    public class QuorumNodeInfo : GenericRpcRequestResponseHandlerNoParam<NodeInfo>, IQuorumNodeInfo
    {
        public QuorumNodeInfo(IClient client) : base(client, ApiMethods.quorum_nodeInfo.ToString())
        {
        }
    }

    public interface IQuorumNodeInfo : IGenericRpcRequestResponseHandlerNoParam<NodeInfo>
    {

    }


}