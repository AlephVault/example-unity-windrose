using System.Threading.Tasks;

namespace AlephVault.Unity.EVMGames.Nethereum.Contracts.DeploymentHandlers
{
    public interface IDeploymentTransactionSenderHandler<TContractDeploymentMessage> where TContractDeploymentMessage : ContractDeploymentMessage, new()
    {
        Task<string> SendTransactionAsync(TContractDeploymentMessage deploymentMessage = null);
    }
}