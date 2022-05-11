using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Contracts.Services;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.BlockProcessing.CrawlerSteps
{
    public class FilterLogCrawlerStep : CrawlerStep<FilterLogVO, FilterLogVO>
    {
        public FilterLogCrawlerStep(IEthApiContractService ethApiContractService) : base(ethApiContractService)
        {
        }

        public override Task<FilterLogVO> GetStepDataAsync(FilterLogVO filterLogVO)
        {
            return Task.FromResult(filterLogVO);
        }
    }
}