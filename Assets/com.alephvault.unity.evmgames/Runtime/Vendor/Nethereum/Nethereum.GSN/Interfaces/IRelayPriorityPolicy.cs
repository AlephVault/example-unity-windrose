using AlephVault.Unity.EVMGames.Nethereum.GSN.Models;
using System.Collections.Generic;

namespace AlephVault.Unity.EVMGames.Nethereum.GSN.Interfaces
{
    public interface IRelayPriorityPolicy
    {
        IEnumerable<RelayOnChain> Execute(IEnumerable<RelayOnChain> relays);
    }
}
