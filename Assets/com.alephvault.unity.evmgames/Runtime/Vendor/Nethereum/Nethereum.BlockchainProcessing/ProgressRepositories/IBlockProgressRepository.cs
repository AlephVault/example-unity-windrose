using System.Numerics;
using System.Threading.Tasks;

namespace AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.ProgressRepositories
{
    public interface IBlockProgressRepository
    {
        Task UpsertProgressAsync(BigInteger blockNumber);
        Task<BigInteger?> GetLastBlockNumberProcessedAsync();
    }
}