using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;
using AlephVault.Unity.EVMGames.Nethereum.Rsk.RPC.RskEth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.Rsk.RPC.RskEth
{
    public interface IRskEthGetBlockWithTransactionsByNumber
    {
        Task<RskBlockWithTransactions> SendRequestAsync(BlockParameter blockParameter, object id = null);
        Task<RskBlockWithTransactions> SendRequestAsync(HexBigInteger number, object id = null);
        RpcRequest BuildRequest(HexBigInteger number, object id = null);
        RpcRequest BuildRequest(BlockParameter blockParameter, object id = null);
    }
}