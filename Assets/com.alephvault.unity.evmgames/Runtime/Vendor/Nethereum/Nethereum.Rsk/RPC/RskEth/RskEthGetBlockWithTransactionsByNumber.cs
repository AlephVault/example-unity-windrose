using System;
using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;
using AlephVault.Unity.EVMGames.Nethereum.Rsk.RPC.RskEth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.Rsk.RPC.RskEth
{
    public class RskEthGetBlockWithTransactionsByNumber : RpcRequestResponseHandler<RskBlockWithTransactions>, IRskEthGetBlockWithTransactionsByNumber
    {
        public RskEthGetBlockWithTransactionsByNumber(IClient client)
            : base(client, ApiMethods.eth_getBlockByNumber.ToString())
        {
        }

        public Task<RskBlockWithTransactions> SendRequestAsync(BlockParameter blockParameter, object id = null)
        {
            if (blockParameter == null) throw new ArgumentNullException(nameof(blockParameter));
            return base.SendRequestAsync(id, blockParameter, true);
        }

        public Task<RskBlockWithTransactions> SendRequestAsync(HexBigInteger number, object id = null)
        {
            if (number == null) throw new ArgumentNullException(nameof(number));
            return base.SendRequestAsync(id, number, true);
        }

        public RpcRequest BuildRequest(HexBigInteger number, object id = null)
        {
            if (number == null) throw new ArgumentNullException(nameof(number));
            return base.BuildRequest(id, number, true);
        }

        public RpcRequest BuildRequest(BlockParameter blockParameter, object id = null)
        {
            if (blockParameter == null) throw new ArgumentNullException(nameof(blockParameter));
            return base.BuildRequest(id, blockParameter, true);
        }
    }
}