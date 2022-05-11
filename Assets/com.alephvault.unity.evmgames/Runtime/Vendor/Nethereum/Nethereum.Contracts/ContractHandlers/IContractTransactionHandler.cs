using System.Threading;
using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.Contracts.ContractHandlers
{
    public interface IContractTransactionHandler<TContractMessage> where TContractMessage : FunctionMessage, new()
    {
        Task<TransactionInput> CreateTransactionInputEstimatingGasAsync(string contractAddress, TContractMessage functionMessage = null);
        Task<HexBigInteger> EstimateGasAsync(string contractAddress, TContractMessage functionMessage = null);
        Task<TransactionReceipt> SendRequestAndWaitForReceiptAsync(string contractAddress, TContractMessage functionMessage = null, CancellationTokenSource tokenSource = null);
        Task<string> SendRequestAsync(string contractAddress, TContractMessage functionMessage = null);
        Task<string> SignTransactionAsync(string contractAddress, TContractMessage functionMessage = null);
    }
}