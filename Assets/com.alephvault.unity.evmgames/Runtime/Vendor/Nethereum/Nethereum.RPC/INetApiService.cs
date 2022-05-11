using AlephVault.Unity.EVMGames.Nethereum.RPC.Net;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC
{
    public interface INetApiService
    {
        INetListening Listening { get; }
        INetPeerCount PeerCount { get; }
        INetVersion Version { get; }
    }
}