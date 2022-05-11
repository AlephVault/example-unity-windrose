using AlephVault.Unity.EVMGames.Nethereum.ABI.FunctionEncoding.Attributes;
using AlephVault.Unity.EVMGames.Nethereum.Contracts.ContractHandlers;
using AlephVault.Unity.EVMGames.Nethereum.Contracts.CQS;
using AlephVault.Unity.EVMGames.Nethereum.Contracts.Services;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC;
using AlephVault.Unity.EVMGames.Nethereum.RPC.TransactionManagers;

namespace AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.Services
{
    public class BlockchainProcessingService : IBlockchainProcessingService
    {
        private readonly IEthApiContractService _ethApiContractService;
        public IBlockchainLogProcessingService Logs { get; }
        public IBlockchainBlockProcessingService Blocks { get; }
    
        public BlockchainProcessingService(IEthApiContractService ethApiContractService)
        {
            _ethApiContractService = ethApiContractService;
            Logs = new BlockchainLogProcessingService(ethApiContractService);
            Blocks = new BlockchainBlockProcessingService(ethApiContractService);
        }

    }
}
