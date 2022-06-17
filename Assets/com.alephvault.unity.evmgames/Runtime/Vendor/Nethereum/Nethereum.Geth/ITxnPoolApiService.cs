using AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.TxnPool;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth
{
    public interface ITxnPoolApiService
    {
        ITxnPoolContent PoolContent { get; }
        ITxnPoolInspect PoolInspect { get; }
        ITxnPoolStatus PoolStatus { get; }
    }
}