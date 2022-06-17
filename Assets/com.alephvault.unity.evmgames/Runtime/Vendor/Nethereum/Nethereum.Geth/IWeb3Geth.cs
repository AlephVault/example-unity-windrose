using AlephVault.Unity.EVMGames.Nethereum.Web3;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth
{
    public interface IWeb3Geth: IWeb3
    {
        IAdminApiService Admin { get; }
        IDebugApiService Debug { get; }
        IGethEthApiService GethEth { get; }
        IMinerApiService Miner { get; }
        ITxnPoolApiService TxnPool { get; }
    }
}