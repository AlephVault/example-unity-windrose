using System.Threading;
using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.Contracts.DeploymentHandlers
{
    public interface IDeploymentTransactionReceiptPollHandler<TContractDeploymentMessage> where TContractDeploymentMessage : ContractDeploymentMessage, new()
    {
        Task<TransactionReceipt> SendTransactionAsync(TContractDeploymentMessage deploymentMessage = null, CancellationTokenSource cancellationTokenSource = null);
    }
}