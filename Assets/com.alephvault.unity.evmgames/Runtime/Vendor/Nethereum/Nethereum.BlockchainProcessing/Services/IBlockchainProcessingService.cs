using System;
using Common.Logging;
using AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.BlockProcessing;
using AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.BlockStorage.Repositories;
using AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.ProgressRepositories;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Blocks;

namespace AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.Services
{
    public interface IBlockchainProcessingService
    {
        IBlockchainLogProcessingService Logs { get; }
        IBlockchainBlockProcessingService Blocks { get; }
    }

    public interface IBlockchainBlockProcessingService
    {

        BlockchainCrawlingProcessor CreateBlockProcessor(
            Action<BlockProcessingSteps> stepsConfiguration,
            uint minimumBlockConfirmations = LastConfirmedBlockNumberService.DEFAULT_BLOCK_CONFIRMATIONS,
            ILog log = null);

        BlockchainCrawlingProcessor CreateBlockProcessor(
            IBlockProgressRepository blockProgressRepository,
            Action<BlockProcessingSteps> stepsConfiguration,
            uint minimumBlockConfirmations = LastConfirmedBlockNumberService.DEFAULT_BLOCK_CONFIRMATIONS,
            ILog log = null);


        BlockchainCrawlingProcessor CreateBlockStorageProcessor(
            IBlockchainStoreRepositoryFactory blockchainStorageFactory,
            uint minimumBlockConfirmations = LastConfirmedBlockNumberService.DEFAULT_BLOCK_CONFIRMATIONS,
            Action<BlockProcessingSteps> configureSteps = null,
            ILog log = null);

        BlockchainCrawlingProcessor CreateBlockStorageProcessor(
            IBlockchainStoreRepositoryFactory blockchainStorageFactory,
            IBlockProgressRepository blockProgressRepository,
            uint minimumBlockConfirmations = LastConfirmedBlockNumberService.DEFAULT_BLOCK_CONFIRMATIONS,
            Action<BlockProcessingSteps> configureSteps = null,
            ILog log = null);
    }
}