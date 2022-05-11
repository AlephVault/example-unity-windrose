using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Contracts.Services;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.BlockProcessing.CrawlerSteps
{
    public class TransactionReceiptCrawlerStep : CrawlerStep<TransactionVO, TransactionReceiptVO>
    {
        public TransactionReceiptCrawlerStep(IEthApiContractService ethApiContractService) : base(ethApiContractService)
        {
        }

        public override async Task<TransactionReceiptVO> GetStepDataAsync(TransactionVO transactionVO)
        {
            var receipt = await EthApi.Transactions
                .GetTransactionReceipt.SendRequestAsync(transactionVO.Transaction.TransactionHash)
                .ConfigureAwait(false);
            return new TransactionReceiptVO(transactionVO.Block, transactionVO.Transaction, receipt, receipt.HasErrors()?? false);
        }
    }
}