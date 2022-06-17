using System;
using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.GSN.Models;

namespace AlephVault.Unity.EVMGames.Nethereum.GSN.Interfaces
{
    public interface IRelayClient
    {
        Task<GetAddrResponse> GetAddrAsync(Uri relayUrl);
        Task<RelayResponse> RelayAsync(Uri relayUrl, RelayRequest request);
    }
}