using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.BlockStorage.Entities;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;

namespace AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.BlockStorage.Repositories
{
    public interface IBlockRepository
    {
        Task UpsertBlockAsync(Nethereum.RPC.Eth.DTOs.Block source);
        Task<IBlockView> FindByBlockNumberAsync(HexBigInteger blockNumber);
    }
}