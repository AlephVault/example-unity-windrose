using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.BlockStorage.Entities;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.BlockStorage.Repositories
{
    public interface IContractRepository
    {
        Task FillCacheAsync();
        Task UpsertAsync(ContractCreationVO contractCreation);
        Task<bool> ExistsAsync(string contractAddress);

        Task<IContractView> FindByAddressAsync(string contractAddress);
        bool IsCached(string contractAddress);
    }
}