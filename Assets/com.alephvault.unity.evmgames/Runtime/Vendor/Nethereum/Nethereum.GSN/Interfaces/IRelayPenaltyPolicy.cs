using AlephVault.Unity.EVMGames.Nethereum.GSN.Models;
using System.Threading.Tasks;

namespace AlephVault.Unity.EVMGames.Nethereum.GSN.Interfaces
{
    public interface IRelayPenaltyPolicy
    {
        Task PenalizeAsync(RelayOnChain relay);
    }
}
