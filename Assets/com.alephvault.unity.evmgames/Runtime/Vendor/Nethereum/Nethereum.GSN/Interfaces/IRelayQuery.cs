using System.Threading.Tasks;

namespace AlephVault.Unity.EVMGames.Nethereum.GSN.Interfaces
{
    public interface IRelayQuery
    {
        Task<RelayCollection> GetAsync(string hubAddress, IRelayPriorityPolicy policy);
    }
}