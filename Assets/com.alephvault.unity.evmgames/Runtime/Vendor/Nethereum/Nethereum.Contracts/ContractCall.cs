using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.ABI.FunctionEncoding;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexConvertors.Extensions;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Transactions;

namespace AlephVault.Unity.EVMGames.Nethereum.Contracts
{
    public class ContractCall
    {
        private readonly IEthCall _ethCall;
        private readonly BlockParameter _defaulBlock;

        public ContractCall(IEthCall ethCall, BlockParameter defaulBlock)
        {
            _ethCall = ethCall;
            _defaulBlock = defaulBlock;
            if (_defaulBlock == null) _defaulBlock = BlockParameter.CreateLatest();
        }

#if !DOTNET35
        public async Task<string> CallAsync(CallInput callInput, BlockParameter block = null)
        {
            try
            {
                if (block == null) block = _defaulBlock;
                return await _ethCall.SendRequestAsync(callInput, block).ConfigureAwait(false);
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