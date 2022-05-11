using Common.Logging;
using AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.BlockProcessing;
using AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.ProgressRepositories;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Blocks;

namespace AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing
{
    public class BlockchainCrawlingProcessor : BlockchainProcessor
    {
        public BlockCrawlOrchestrator Orchestrator => (BlockCrawlOrchestrator)BlockchainProcessingOrchestrator;
        public BlockchainCrawlingProcessor(BlockCrawlOrchestrator blockchainProcessingOrchestrator, IBlockProgressRepository blockProgressRepository, ILastConfirmedBlockNumberService lastConfirmedBlockNumberService, ILog log = null):base(blockchainProcessingOrchestrator, blockProgressRepository, lastConfirmedBlockNumberService, log)
        {
            
        }
    }
}