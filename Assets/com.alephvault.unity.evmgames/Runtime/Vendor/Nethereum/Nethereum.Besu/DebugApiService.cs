using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.Besu.RPC.Debug;
using AlephVault.Unity.EVMGames.Nethereum.RPC;

namespace AlephVault.Unity.EVMGames.Nethereum.Besu
{
    public class DebugApiService : RpcClientWrapper, IDebugApiService
    {
        public DebugApiService(IClient client) : base(client)
        {
            DebugStorageRangeAt = new DebugStorageRangeAt(client);
            DebugTraceTransaction = new DebugTraceTransaction(client);
            DebugMetrics = new DebugMetrics(client);
        }

        public IDebugStorageRangeAt DebugStorageRangeAt { get; }
        public IDebugTraceTransaction DebugTraceTransaction { get; }
        public IDebugMetrics DebugMetrics { get; }
    }
}