using System.Numerics;
using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Contracts.Services;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.BlockProcessing.CrawlerSteps
{
    public class BlockCrawlerStep : CrawlerStep<BigInteger, BlockWithTransactions>
    {
        public BlockCrawlerStep(IEthApiContractService ethApiContractService) : base(ethApiContractService)
        {

        }
        public override Task<BlockWithTransactions> GetStepDataAsync(BigInteger blockNumber)
        {
            return EthApi.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(blockNumber.ToHexBigInteger());
        }
    }
}