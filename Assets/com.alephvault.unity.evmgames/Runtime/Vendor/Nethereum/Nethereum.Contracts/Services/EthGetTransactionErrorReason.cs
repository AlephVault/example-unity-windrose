using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.ABI.FunctionEncoding;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexConvertors.Extensions;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Services;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Transactions;

namespace AlephVault.Unity.EVMGames.Nethereum.Contracts.Services
{
    public class EthGetContractTransactionErrorReason: IEthGetContractTransactionErrorReason
    {
        private readonly IEthApiTransactionsService _apiTransactionsService;

        public EthGetContractTransactionErrorReason(IEthApiTransactionsService apiTransactionsService)
        {
            _apiTransactionsService = apiTransactionsService;
        }
#if !DOTNET35
        public async Task<string> SendRequestAsync(string transactionHash)
        {
            var transaction = await _apiTransactionsService.GetTransactionByHash.SendRequestAsync(transactionHash);
            var transactionInput = transaction.ConvertToTransactionInput();
            var functionCallDecoder = new FunctionCallDecoder();
            if (transactionInput.MaxFeePerGas != null)
            {
                transactionInput.GasPrice = null;
            }
            try
            {
                var errorHex = await _apiTransactionsService.Call.SendRequestAsync(transactionInput, new BlockParameter(transaction.BlockNumber));

                if (ErrorFunction.IsErrorData(errorHex))
                {
                    return functionCallDecoder.DecodeFunctionErrorMessage(errorHex);
                }
                return string.Empty;

            }
            catch (RpcResponseException rpcException)
            {
                ContractRevertExceptionHandler.HandleContractRevertException(rpcException);
                throw;
            }
        }
#endif
    }
}