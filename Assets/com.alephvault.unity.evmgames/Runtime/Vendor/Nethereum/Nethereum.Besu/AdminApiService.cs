using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.Besu.RPC.Admin;
using AlephVault.Unity.EVMGames.Nethereum.RPC;


namespace AlephVault.Unity.EVMGames.Nethereum.Besu
{
    public class AdminApiService : RpcClientWrapper, IAdminApiService
    {
        public AdminApiService(IClient client) : base(client)
        {
            AddPeer = new AdminAddPeer(client);
            NodeInfo = new AdminNodeInfo(client);
            Peers = new AdminPeers(client);
            RemovePeer = new AdminRemovePeer(client);
        }

        public IAdminAddPeer AddPeer { get; }
        public IAdminNodeInfo NodeInfo { get; }
        public IAdminPeers Peers { get; }
        public IAdminRemovePeer RemovePeer { get; }
    }
}