using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.BlockStorage.Repositories;
using AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.Processor;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.BlockStorage.BlockStorageStepsHandlers
{
    public class ContractCreationStorageStepHandler : ProcessorBaseHandler<ContractCreationVO>
    {
        private readonly IContractRepository _contractRepository;
        public ContractCreationStorageStepHandler(IContractRepository contractRepository)
        {
            _contractRepository = contractRepository;
        }
        protected override Task ExecuteInternalAsync(ContractCreationVO contractCreation)
        {
            return _contractRepository.UpsertAsync(
                contractCreation);
        }
    }
}
