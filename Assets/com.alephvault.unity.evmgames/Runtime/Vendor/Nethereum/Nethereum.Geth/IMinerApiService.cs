using AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Miner;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth
{
    public interface IMinerApiService
    {
        IMinerSetGasPrice SetGasPrice { get; }
        IMinerStart Start { get; }
        IMinerStop Stop { get; }
    }
}