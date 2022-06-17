using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexConvertors.Extensions;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC;
using AlephVault.Unity.EVMGames.Nethereum.Rsk.RPC.RskEth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.Rsk.RPC.RskEth
{
    public class RskEthGetBlockWithTransactionsByHash : RpcRequestResponseHandler<RskBlockWithTransactions>, IRskEthGetBlockWithTransactionsByHash
    {
        public RskEthGetBlockWithTransactionsByHash(IClient client)
            : base(client, ApiMethods.eth_getBlockByHash.ToString())
        {
        }

        public Task<RskBlockWithTransactions> SendRequestAsync(string blockHash, object id = null)
        {
            return base.SendRequestAsync(id, blockHash.EnsureHexPrefix(), true);
        }

        public RpcRequest BuildRequest(string blockHash, object id = null)
        {
            return base.BuildRequest(id, blockHash.EnsureHexPrefix(), true);
        }
    }
}