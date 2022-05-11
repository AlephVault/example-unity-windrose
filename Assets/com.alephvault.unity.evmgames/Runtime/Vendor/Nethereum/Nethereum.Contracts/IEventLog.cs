using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.Contracts
{
    public interface IEventLog
    {
        FilterLog Log { get; }
    }
}