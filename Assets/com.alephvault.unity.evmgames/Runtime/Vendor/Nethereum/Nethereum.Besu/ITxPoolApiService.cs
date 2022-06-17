using AlephVault.Unity.EVMGames.Nethereum.Besu.RPC.Txpool;

namespace AlephVault.Unity.EVMGames.Nethereum.Besu
{
    public interface ITxPoolApiService
    {
        ITxpoolBesuStatistics BesuStatistics { get; }
        ITxpoolBesuTransactions BesuTransactions { get; }
    }
}