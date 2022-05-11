using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.ABI.FunctionEncoding.Attributes;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.Contracts.ContractHandlers
{
    public interface IContractQueryHandler<TFunctionMessage> where TFunctionMessage : FunctionMessage, new()
    {
        Task<TFunctionOutput> QueryAsync<TFunctionOutput>(string contractAddress, TFunctionMessage functionMessage = null, BlockParameter block = null);
        Task<TFunctionOutput> QueryDeserializingToObjectAsync<TFunctionOutput>(TFunctionMessage functionMessage, string contractAddress, BlockParameter block = null) where TFunctionOutput : IFunctionOutputDTO, new();
        Task<byte[]> QueryRawAsBytesAsync(string contractAddress, TFunctionMessage functionMessage = null, BlockParameter block = null);
        Task<string> QueryRawAsync(string contractAddress, TFunctionMessage functionMessage = null, BlockParameter block = null);
    }
}