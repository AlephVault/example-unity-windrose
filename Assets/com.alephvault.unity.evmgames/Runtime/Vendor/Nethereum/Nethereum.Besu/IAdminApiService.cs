using AlephVault.Unity.EVMGames.Nethereum.Besu.RPC.Admin;

namespace AlephVault.Unity.EVMGames.Nethereum.Besu
{
    public interface IAdminApiService
    {
        IAdminAddPeer AddPeer { get; }
        IAdminNodeInfo NodeInfo { get; }
        IAdminPeers Peers { get; }
        IAdminRemovePeer RemovePeer { get; }
    }
}