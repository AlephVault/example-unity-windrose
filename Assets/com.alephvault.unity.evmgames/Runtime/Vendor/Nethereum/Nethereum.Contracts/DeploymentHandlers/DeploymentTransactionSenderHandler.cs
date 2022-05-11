using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Contracts.Extensions;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using AlephVault.Unity.EVMGames.Nethereum.RPC.TransactionManagers;

namespace AlephVault.Unity.EVMGames.Nethereum.Contracts.DeploymentHandlers
{
#if !DOTNET35
    public class DeploymentTransactionSenderHandler<TContractDeploymentMessage> : DeploymentHandlerBase<TContractDeploymentMessage>, 
        IDeploymentTransactionSenderHandler<TContractDeploymentMessage> where TContractDeploymentMessage : ContractDeploymentMessage, new()
    {
        private readonly IDeploymentEstimatorHandler<TContractDeploymentMessage> _deploymentEstimatorHandler;

        public DeploymentTransactionSenderHandler(ITransactionManager transactionManager):base(transactionManager)
        {
            _deploymentEstimatorHandler = new DeploymentEstimatorHandler<TContractDeploymentMessage>(transactionManager);
        }

        public async Task<string> SendTransactionAsync(TContractDeploymentMessage deploymentMessage = null)
        {
            if(deploymentMessage == null) deploymentMessage = new TContractDeploymentMessage();
            deploymentMessage.Gas = await GetOrEstimateMaximumGasAsync(deploymentMessage).ConfigureAwait(false);
            var transactionInput = DeploymentMessageEncodingService.CreateTransactionInput(deploymentMessage);
            return await TransactionManager.SendTransactionAsync(transactionInput).ConfigureAwait(false);
        }

        protected virtual async Task<HexBigInteger> GetOrEstimateMaximumGasAsync(
            TContractDeploymentMessage deploymentMessage)
        {
            return deploymentMessage.GetHexMaximumGas()
                   ?? await _deploymentEstimatorHandler.EstimateGasAsync(deploymentMessage).ConfigureAwait(false);
        }
    }
#endif
}