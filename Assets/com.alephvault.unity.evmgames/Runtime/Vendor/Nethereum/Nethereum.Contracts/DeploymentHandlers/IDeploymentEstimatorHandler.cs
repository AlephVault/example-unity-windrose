using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;

namespace AlephVault.Unity.EVMGames.Nethereum.Contracts.DeploymentHandlers
{
    public interface IDeploymentEstimatorHandler<TContractDeploymentMessage> where TContractDeploymentMessage : ContractDeploymentMessage, new()
    {
        Task<HexBigInteger> EstimateGasAsync(TContractDeploymentMessage deploymentMessage);
    }
}