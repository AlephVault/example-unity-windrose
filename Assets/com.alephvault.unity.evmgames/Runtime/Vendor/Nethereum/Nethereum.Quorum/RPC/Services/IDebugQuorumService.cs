using AlephVault.Unity.EVMGames.Nethereum.Quorum.RPC.Debug;

namespace AlephVault.Unity.EVMGames.Nethereum.Quorum.RPC.Services
{
    public interface IDebugQuorumService
    {
        IDebugDumpAddress DebugDumpAddress { get; }
        IDebugPrivateStateRoot DebugPrivateStateRoot { get; }
    }
}