using System.Threading;
using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.Contracts.CQS
{
    public interface IContractDeploymentTransactionHandler<TContractDeploymentMessage> where TContractDeploymentMessage : ContractDeploymentMessage, new()
    {
        Task<TransactionInput> CreateTransactionInputEstimatingGasAsync(TContractDeploymentMessage deploymentMessage = null);
        Task<HexBigInteger> EstimateGasAsync(TContractDeploymentMessage contractDeploymentMessage);
        Task<TransactionReceipt> SendRequestAndWaitForReceiptAsync(TContractDeploymentMessage contractDeploymentMessage = null, CancellationTokenSource tokenSource = null);
        Task<string> SendRequestAsync(TContractDeploymentMessage contractDeploymentMessage = null);
        Task<string> SignTransactionAsync(TContractDeploymentMessage contractDeploymentMessage);
    }
}