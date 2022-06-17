using System;
using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexConvertors.Extensions;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Blocks;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;
using AlephVault.Unity.EVMGames.Nethereum.Rsk.RPC.RskEth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.Rsk.RPC.RskEth
{
    public class RskEthGetBlockWithTransactionsHashesByHash : RpcRequestResponseHandler<RskBlockWithTransactionHashes>, IRskEthGetBlockWithTransactionsHashesByHash
    {
        public RskEthGetBlockWithTransactionsHashesByHash(IClient client)
            : base(client, ApiMethods.eth_getBlockByHash.ToString())
        {
        }

        public Task<RskBlockWithTransactionHashes> SendRequestAsync(string blockHash, object id = null)
        {
            if (blockHash == null) throw new ArgumentNullException(nameof(blockHash));
            return base.SendRequestAsync(id, blockHash.EnsureHexPrefix(), false);
        }

        public RpcRequest BuildRequest(string blockHash, object id = null)
        {
            if (blockHash == null) throw new ArgumentNullException(nameof(blockHash));
            return base.BuildRequest(id, blockHash.EnsureHexPrefix(), false);
        }
    }
}