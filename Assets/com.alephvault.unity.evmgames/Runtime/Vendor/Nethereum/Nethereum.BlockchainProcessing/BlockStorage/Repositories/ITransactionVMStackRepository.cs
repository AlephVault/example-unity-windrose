using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.BlockStorage.Entities;
using Newtonsoft.Json.Linq;

namespace AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.BlockStorage.Repositories
{
    public interface ITransactionVMStackRepository
    { 
        Task UpsertAsync(string transactionHash, string address, JObject stackTrace);
        Task<ITransactionVmStackView> FindByTransactionHashAsync(string hash);
        Task<ITransactionVmStackView> FindByAddressAndTransactionHashAsync(string address, string hash);
    }
}