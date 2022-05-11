using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;

namespace AlephVault.Unity.EVMGames.Nethereum.Contracts.TransactionHandlers
{
    public interface ITransactionEstimatorHandler<TFunctionMessage> where TFunctionMessage : FunctionMessage, new()
    {
        Task<HexBigInteger> EstimateGasAsync(string contractAddress, TFunctionMessage functionMessage = null);
    }
}