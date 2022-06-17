using AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.GethEth;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth
{
    public interface IGethEthApiService
    {
        IEthPendingTransactions PendingTransactions { get; }
        IEthCall Call { get; }
    }
}