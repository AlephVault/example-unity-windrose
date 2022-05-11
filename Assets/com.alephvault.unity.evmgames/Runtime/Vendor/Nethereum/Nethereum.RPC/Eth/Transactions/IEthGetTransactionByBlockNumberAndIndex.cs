using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Transactions
{
    public interface IEthGetTransactionByBlockNumberAndIndex
    {
        RpcRequest BuildRequest(HexBigInteger blockNumber, HexBigInteger transactionIndex, object id = null);
        Task<Transaction> SendRequestAsync(HexBigInteger blockNumber, HexBigInteger transactionIndex, object id = null);
    }
}