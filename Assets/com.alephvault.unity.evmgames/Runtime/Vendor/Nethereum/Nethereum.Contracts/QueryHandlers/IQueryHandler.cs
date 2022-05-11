using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.Contracts.QueryHandlers
{
    public interface IQueryHandler<TFunctionMessage, TOutput> 
        where TFunctionMessage : FunctionMessage, new()
    {
        Task<TOutput> QueryAsync(
             string contractAddress,
             TFunctionMessage functionMessage = null,
             BlockParameter block = null);
    }
}