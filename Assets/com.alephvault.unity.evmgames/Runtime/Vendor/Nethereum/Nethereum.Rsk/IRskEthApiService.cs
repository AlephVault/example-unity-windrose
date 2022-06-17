using AlephVault.Unity.EVMGames.Nethereum.Rsk.RPC.RskEth;

namespace AlephVault.Unity.EVMGames.Nethereum.Rsk
{
    public interface IRskEthApiService
    {
        IRskEthGetBlockWithTransactionsByHash GetBlockWithTransactionsByHash { get; }
        IRskEthGetBlockWithTransactionsByNumber GetBlockWithTransactionsByNumber { get; }
        IRskEthGetBlockWithTransactionsHashesByHash GetBlockWithTransactionsHashesByHash { get; }
        IRskEthGetBlockWithTransactionsHashesByNumber GetBlockWithTransactionsHashesByNumber { get; }
    }
}