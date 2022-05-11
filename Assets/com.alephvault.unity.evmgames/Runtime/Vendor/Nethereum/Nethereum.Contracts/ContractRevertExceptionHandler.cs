using System;
using AlephVault.Unity.EVMGames.Nethereum.ABI.FunctionEncoding;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexConvertors.Extensions;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Transactions;
using Newtonsoft.Json.Linq;

namespace AlephVault.Unity.EVMGames.Nethereum.Contracts
{
    public class ContractRevertExceptionHandler
    {
        public static void HandleContractRevertException(RpcResponseException rpcException)
        {
            if (rpcException.RpcError.Data != null)
            {
                var encodedErrorData = rpcException.RpcError.Data.ToString();
                if (encodedErrorData.IsHex())
                {
                    //check normal revert
                    new FunctionCallDecoder().ThrowIfErrorOnOutput(encodedErrorData);

                    //throw custom error
                    throw new SmartContractCustomErrorRevertException(encodedErrorData);
                }
            }
        }

       
    }
}