using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Transactions
{
    public interface IEthEstimateGas
    {
        RpcRequest BuildRequest(CallInput callInput, object id = null);
        Task<HexBigInteger> SendRequestAsync(CallInput callInput, object id = null);
    }
}