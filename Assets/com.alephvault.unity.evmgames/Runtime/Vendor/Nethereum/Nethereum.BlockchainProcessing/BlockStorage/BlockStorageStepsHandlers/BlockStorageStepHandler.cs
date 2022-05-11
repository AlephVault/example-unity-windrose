using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.BlockStorage.Repositories;
using AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.Processor;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.BlockStorage.BlockStorageStepsHandlers
{
    public class BlockStorageStepHandler : ProcessorBaseHandler<BlockWithTransactions>
    {
        private readonly IBlockRepository _blockRepository;

        public BlockStorageStepHandler(IBlockRepository blockRepository)
        {
            _blockRepository = blockRepository;
        }
        protected override Task ExecuteInternalAsync(BlockWithTransactions block)
        {
            return _blockRepository.UpsertBlockAsync(block);
        }
    }
}
