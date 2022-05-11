using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.BlockStorage.Entities;
using AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.BlockStorage.Entities.Mapping;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;
using AlephVault.Unity.EVMGames.Nethereum.Util;

namespace AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.BlockStorage.Repositories
{
    public class InMemoryAddressTransactionRepository : IAddressTransactionRepository
    {
        public InMemoryAddressTransactionRepository(List<IAddressTransactionView> records)
        {
            Records = records;
        }

        public List<IAddressTransactionView> Records { get; set; }

        public Task<IAddressTransactionView> FindAsync(string address, HexBigInteger blockNumber, string transactionHash)
        {
            IAddressTransactionView result = Records.FirstOrDefault(
                t => t.BlockNumber == blockNumber.Value.ToString() 
                && t.Hash == transactionHash 
                && AddressUtil.Current.AreAddressesTheSame(t.Address, address));

            return Task.FromResult(result);
        }

        public async Task UpsertAsync(TransactionReceiptVO transactionReceiptVO, string address, string error = null, string newContractAddress = null)
        {
            var entity = await FindAsync(address, transactionReceiptVO.Transaction.BlockNumber, transactionReceiptVO.TransactionHash).ConfigureAwait(false);
            if(entity != null) Records.Remove(entity);
            Records.Add(transactionReceiptVO.MapToStorageEntityForUpsert(address));
        }
    }
}
