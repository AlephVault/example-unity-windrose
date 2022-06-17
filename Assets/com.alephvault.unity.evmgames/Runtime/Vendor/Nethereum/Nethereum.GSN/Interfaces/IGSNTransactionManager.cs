using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.GSN.Interfaces
{
    public interface IGSNTransactionManager
    {
        Task<string> SendTransactionAsync(TransactionInput transactionInput);
    }
}