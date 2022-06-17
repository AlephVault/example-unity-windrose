using AlephVault.Unity.EVMGames.Nethereum.Quorum.RPC.ContractExtensions;

namespace AlephVault.Unity.EVMGames.Nethereum.Quorum.RPC.Services
{
    public interface IContractExtensionsService
    {
        IQuorumExtensionActiveExtensionContracts ActiveExtensionContracts { get; }
        IQuorumExtensionApproveExtension ApproveExtension { get; }
        IQuorumExtensionCancelExtension CancelExtension { get; }
        IQuorumExtensionExtendContract ExtendContract { get; }
        IQuorumExtensionGetExtensionStatus GetExtensionStatus { get; }
    }
}