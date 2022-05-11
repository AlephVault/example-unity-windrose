using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.BlockStorage.Repositories
{
    public interface ITransactionRepository
    {
        Task UpsertAsync(TransactionReceiptVO transactionReceiptVO, string code, bool failedCreatingContract);

        Task UpsertAsync(TransactionReceiptVO transactionReceiptVO);

        Task<Entities.ITransactionView> FindByBlockNumberAndHashAsync(HexBigInteger blockNumber, string hash);
    }
}