using AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Admin;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth
{
    public class AdminApiService : RpcClientWrapper, IAdminApiService
    {
        public AdminApiService(IClient client) : base(client)
        {
            AddPeer = new AdminAddPeer(client);
            Datadir = new AdminDatadir(client);
            NodeInfo = new AdminNodeInfo(client);
            StartRPC = new AdminStartRPC(client);
            StartWS = new AdminStartWS(client);
            StopRPC = new AdminStopRPC(client);
            StopWS = new AdminStopWS(client);
            Peers = new AdminPeers(client);
            RemovePeer = new AdminRemovePeer(client);
            AddTrustedPeer = new AdminAddTrustedPeer(client);
            RemoveTrustedPeer = new AdminRemoveTrustedPeer(client);
            StartHttp = new AdminStartHTTP(client);
            StopHttp = new AdminStopHTTP(client);
            ExportChain = new AdminExportChain(client);
            ImportChain = new AdminImportChain(client);
        }

        public IAdminAddPeer AddPeer { get; }
        public IAdminRemovePeer RemovePeer { get; }
        public IAdminAddTrustedPeer AddTrustedPeer { get; }
        public IAdminRemoveTrustedPeer RemoveTrustedPeer { get; }
        public IAdminStartHTTP StartHttp { get; }
        public IAdminStopHTTP StopHttp { get; }
        public IAdminExportChain ExportChain { get; }
        public IAdminImportChain ImportChain { get; }
        public IAdminDatadir Datadir { get; }
        public IAdminNodeInfo NodeInfo { get; }
        public IAdminStartRPC StartRPC { get; }
        public IAdminStartWS StartWS { get; }
        public IAdminStopRPC StopRPC { get; }
        public IAdminStopWS StopWS { get; }
        public IAdminPeers Peers { get; }
    }
}