using AlephVault.Unity.EVMGames.Nethereum.GSN.Models;
using System.Threading.Tasks;

namespace AlephVault.Unity.EVMGames.Nethereum.GSN.Interfaces
{
    public interface IRelayGracePolicy
    {
        Task GraceAsync(RelayOnChain relay);
    }
}
