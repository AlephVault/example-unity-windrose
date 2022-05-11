using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.BlockStorage.Entities;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.BlockStorage.Repositories
{
    public interface IAddressTransactionRepository
    {
        Task UpsertAsync(
            TransactionReceiptVO transactionReceiptVO, string address, string error = null, 
            string newContractAddress = null);

        Task<IAddressTransactionView> FindAsync(
            string address, HexBigInteger blockNumber, string transactionHash);
    }
}