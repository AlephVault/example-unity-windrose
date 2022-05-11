using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Contracts.Services;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.BlockProcessing.CrawlerSteps
{
    public class TransactionCrawlerStep : CrawlerStep<TransactionVO, TransactionVO>
    {
        public TransactionCrawlerStep(IEthApiContractService ethApiContractService) : base(ethApiContractService)
        {
        }

        public override Task<TransactionVO> GetStepDataAsync(TransactionVO parentStep)
        {
            return Task.FromResult(parentStep);
        }
    }
}