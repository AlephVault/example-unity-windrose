using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;
using System.Numerics;
using System.Threading;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Accounts;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Fee1559Suggestions;
using AlephVault.Unity.EVMGames.Nethereum.RPC.TransactionReceipts;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.TransactionManagers
{
    public interface ITransactionManager
    {
        IClient Client { get; set; }
        BigInteger DefaultGasPrice { get; set; }
        BigInteger DefaultGas { get; set; }
        IAccount Account { get; }
        bool UseLegacyAsDefault { get; set; }
#if !DOTNET35
        IFee1559SuggestionStrategy Fee1559SuggestionStrategy { get; set; }

        Task<string> SendTransactionAsync(TransactionInput transactionInput);
        Task<HexBigInteger> EstimateGasAsync(CallInput callInput);
        Task<string> SendTransactionAsync(string from, string to, HexBigInteger amount);
        Task<string> SignTransactionAsync(TransactionInput transaction);
        ITransactionReceiptService TransactionReceiptService { get; set; }
        Task<TransactionReceipt> SendTransactionAndWaitForReceiptAsync(TransactionInput transactionInput, CancellationTokenSource tokenSource);
#endif

    }
}
