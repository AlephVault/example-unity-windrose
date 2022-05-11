using System.Threading.Tasks;

namespace AlephVault.Unity.EVMGames.Nethereum.Contracts.DeploymentHandlers
{
    public interface IDeploymentSigner<TContractDeploymentMessage> where TContractDeploymentMessage : ContractDeploymentMessage, new()
    {
        Task<string> SignTransactionAsync(TContractDeploymentMessage deploymentMessage);
    }
}