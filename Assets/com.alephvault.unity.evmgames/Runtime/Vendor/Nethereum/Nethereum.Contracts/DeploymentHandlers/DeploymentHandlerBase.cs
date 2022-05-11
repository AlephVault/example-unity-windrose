using AlephVault.Unity.EVMGames.Nethereum.Contracts.ContractHandlers;
using AlephVault.Unity.EVMGames.Nethereum.Contracts.MessageEncodingServices;
using AlephVault.Unity.EVMGames.Nethereum.RPC.TransactionManagers;

namespace AlephVault.Unity.EVMGames.Nethereum.Contracts.DeploymentHandlers
{
#if !DOTNET35
    public abstract class DeploymentHandlerBase<TContractDeploymentMessage> : ContractTransactionHandlerBase
        where TContractDeploymentMessage : ContractDeploymentMessage, new()
    {
        protected DeploymentMessageEncodingService<TContractDeploymentMessage> DeploymentMessageEncodingService { get; set;}

        private void InitialiseEncodingService()
        {
            DeploymentMessageEncodingService = new DeploymentMessageEncodingService<TContractDeploymentMessage>();
            DeploymentMessageEncodingService.DefaultAddressFrom = GetAccountAddressFrom();
        }

        protected DeploymentHandlerBase(ITransactionManager transactionManager) : base(transactionManager)
        {
            InitialiseEncodingService();
        }
    }
#endif
}