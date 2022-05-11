using System.Threading.Tasks;

namespace AlephVault.Unity.EVMGames.Nethereum.Util
{
    public interface IWaitStrategy
    {
        Task ApplyAsync(uint retryCount);
    }
}