using AlephVault.Unity.EVMGames.Nethereum.Besu.RPC.Debug;

namespace AlephVault.Unity.EVMGames.Nethereum.Besu
{
    public interface IDebugApiService
    {
        IDebugStorageRangeAt DebugStorageRangeAt { get; }
        IDebugTraceTransaction DebugTraceTransaction { get; }
        IDebugMetrics DebugMetrics { get; }
    }
}