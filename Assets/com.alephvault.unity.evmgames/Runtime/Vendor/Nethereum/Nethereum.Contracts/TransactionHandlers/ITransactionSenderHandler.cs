using System.Threading.Tasks;

namespace AlephVault.Unity.EVMGames.Nethereum.Contracts.TransactionHandlers
{
    public interface ITransactionSenderHandler<TFunctionMessage> where TFunctionMessage : FunctionMessage, new()
    {
        Task<string> SendTransactionAsync(string contractAddress, TFunctionMessage functionMessage = null);
    }
}