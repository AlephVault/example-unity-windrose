using System.Threading.Tasks;

namespace AlephVault.Unity.EVMGames.Nethereum.Contracts.TransactionHandlers
{
    public interface ITransactionSigner<TFunctionMessage> where TFunctionMessage : FunctionMessage, new()
    {
        Task<string> SignTransactionAsync(string contractAddress, TFunctionMessage functionMessage = null);
    }
}