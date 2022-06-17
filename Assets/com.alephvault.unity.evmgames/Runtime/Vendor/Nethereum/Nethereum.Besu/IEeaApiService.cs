using AlephVault.Unity.EVMGames.Nethereum.Besu.RPC.EEA;

namespace AlephVault.Unity.EVMGames.Nethereum.Besu
{
    public interface IEeaApiService
    {
        IEeaGetTransactionReceipt GetTransactionReceipt { get; }
        IEeaSendRawTransaction SendRawTransaction { get; }
    }
}