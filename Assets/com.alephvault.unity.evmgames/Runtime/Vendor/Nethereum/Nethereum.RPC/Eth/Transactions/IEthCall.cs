using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Transactions
{
    public interface IEthCall
    {
        BlockParameter DefaultBlock { get; set; }

        RpcRequest BuildRequest(CallInput callInput, BlockParameter block, object id = null);
        Task<string> SendRequestAsync(CallInput callInput, object id = null);
        Task<string> SendRequestAsync(CallInput callInput, BlockParameter block, object id = null);
    }
}