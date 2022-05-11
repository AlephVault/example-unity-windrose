using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.BlockStorage.Repositories;
using AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.Processor;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.BlockStorage.BlockStorageStepsHandlers
{
    public class FilterLogStorageStepHandler : ProcessorBaseHandler<FilterLogVO>
    {
        private readonly ITransactionLogRepository _transactionLogRepository;

        public FilterLogStorageStepHandler(ITransactionLogRepository transactionLogRepository)
        {
            _transactionLogRepository = transactionLogRepository;
        }

        protected override Task ExecuteInternalAsync(FilterLogVO filterLog)
        {
            return _transactionLogRepository.UpsertAsync(filterLog);
        }
    }
}
