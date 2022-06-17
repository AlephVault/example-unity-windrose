using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.Quorum.RPC.ContractExtensions;
using AlephVault.Unity.EVMGames.Nethereum.RPC;

namespace AlephVault.Unity.EVMGames.Nethereum.Quorum.RPC.Services
{
    public class ContractExtensionsService: RpcClientWrapper, IContractExtensionsService
    {
        public ContractExtensionsService(IClient client) : base(client)
        {
            ActiveExtensionContracts = new QuorumExtensionActiveExtensionContracts(client);
            ApproveExtension = new QuorumExtensionApproveExtension(client);
            CancelExtension = new QuorumExtensionCancelExtension(client);
            ExtendContract = new QuorumExtensionExtendContract(client);
            GetExtensionStatus = new QuorumExtensionGetExtensionStatus(client);
        }

        public IQuorumExtensionActiveExtensionContracts ActiveExtensionContracts { get; }
        public IQuorumExtensionApproveExtension ApproveExtension { get; }
        public IQuorumExtensionCancelExtension CancelExtension { get; }
        public IQuorumExtensionExtendContract ExtendContract { get; }
        public IQuorumExtensionGetExtensionStatus GetExtensionStatus { get; }
    }
}