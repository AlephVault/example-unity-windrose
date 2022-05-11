using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.BlockStorage.Entities;
using AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.BlockStorage.Entities.Mapping;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.BlockStorage.Repositories
{
    public class InMemoryTransactionLogRepository : ITransactionLogRepository
    {
        public List<ITransactionLogView> Records { get; set; } 

        public InMemoryTransactionLogRepository(List<ITransactionLogView> records)
        {
            Records = records;
        }

        public Task<ITransactionLogView> FindByTransactionHashAndLogIndexAsync(string hash, BigInteger logIndex)
        {
            return Task.FromResult(Records.FirstOrDefault(r => r.TransactionHash == hash && r.LogIndex == logIndex.ToString()));
        }

        public async Task UpsertAsync(FilterLogVO log)
        {
            var record = await FindByTransactionHashAndLogIndexAsync(log.Transaction.TransactionHash, log.Log.LogIndex).ConfigureAwait(false);
            if(record != null) Records.Remove(record);
            Records.Add(log.MapToStorageEntityForUpsert());
        }
    }
}
