using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.Quorum.RPC.Debug;
using AlephVault.Unity.EVMGames.Nethereum.RPC;

namespace AlephVault.Unity.EVMGames.Nethereum.Quorum.RPC.Services
{
    public class DebugQuorumService : RpcClientWrapper, IDebugQuorumService
    {
        public DebugQuorumService(IClient client):base(client)
        {
            DebugDumpAddress = new DebugDumpAddress(client);
            DebugPrivateStateRoot = new DebugPrivateStateRoot(client);
        }

        public IDebugDumpAddress DebugDumpAddress { get; }
        public IDebugPrivateStateRoot DebugPrivateStateRoot { get; }
    }
}