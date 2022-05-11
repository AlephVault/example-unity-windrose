using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.BlockStorage.Repositories;
using AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.Processor;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.BlockStorage.BlockStorageStepsHandlers
{
    public class TransactionReceiptStorageStepHandler : ProcessorBaseHandler<TransactionReceiptVO>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAddressTransactionRepository _addressTransactionRepository;

        public TransactionReceiptStorageStepHandler(ITransactionRepository transactionRepository, IAddressTransactionRepository addressTransactionRepository = null)
        {
            _transactionRepository = transactionRepository;
            _addressTransactionRepository = addressTransactionRepository;
        }

        protected override async Task ExecuteInternalAsync(TransactionReceiptVO transactionReceiptVO)
        {
            await _transactionRepository.UpsertAsync(transactionReceiptVO).ConfigureAwait(false);

            if(_addressTransactionRepository != null)
            {
                var newContractAddress = transactionReceiptVO.IsForContractCreation() ? transactionReceiptVO.TransactionReceipt.ContractAddress : string.Empty;
                foreach (var address in transactionReceiptVO.GetAllRelatedAddresses())
                {
                    await _addressTransactionRepository.UpsertAsync(transactionReceiptVO,
                                                                    address, 
                                                                    null, 
                                                                    newContractAddress).ConfigureAwait(false);
                }
            }
        }
    }
}
