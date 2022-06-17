using AlephVault.Unity.EVMGames.Nethereum.Parity.RPC.Network;

namespace AlephVault.Unity.EVMGames.Nethereum.Parity
{
    public interface INetworkApiService
    {
        IParityChainStatus ChainStatus { get; }
        IParityGasPriceHistogram GasPriceHistogram { get; }
        IParityNetPeers NetPeers { get; }
        IParityNetPort NetPort { get; }
        IParityPendingTransactions PendingTransactions { get; }
    }
}