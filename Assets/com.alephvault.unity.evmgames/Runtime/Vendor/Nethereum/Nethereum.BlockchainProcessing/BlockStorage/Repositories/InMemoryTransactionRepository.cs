using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.BlockStorage.Entities;
using AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.BlockStorage.Entities.Mapping;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.BlockStorage.Repositories
{
    public class InMemoryTransactionRepository : ITransactionRepository
    {
        public List<ITransactionView> Records { get; set;}

        public InMemoryTransactionRepository(List<ITransactionView> records)
        {
            Records = records;
        }

        public Task<ITransactionView> FindByBlockNumberAndHashAsync(HexBigInteger blockNumber, string hash)
        {
            return Task.FromResult(Records.FirstOrDefault(r => r.BlockNumber == blockNumber.Value.ToString() && r.Hash == hash));
        }

        public async Task UpsertAsync(TransactionReceiptVO transactionReceiptVO, string code, bool failedCreatingContract)
        {
            var record = await FindByBlockNumberAndHashAsync(transactionReceiptVO.BlockNumber, transactionReceiptVO.TransactionHash).ConfigureAwait(false);
            if(record != null ) Records.Remove(record);
            Records.Add(transactionReceiptVO.MapToStorageEntityForUpsert(code, failedCreatingContract));
        }

        public async Task UpsertAsync(TransactionReceiptVO transactionReceiptVO)
        {
            var record = await FindByBlockNumberAndHashAsync(transactionReceiptVO.BlockNumber, transactionReceiptVO.TransactionHash).ConfigureAwait(false);
            if (record != null) Records.Remove(record);
            Records.Add(transactionReceiptVO.MapToStorageEntityForUpsert());
        }
    }
}
