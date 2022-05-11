using System;
using System.Threading.Tasks;
 
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Transactions
{
    public class EthSendTransaction : RpcRequestResponseHandler<string>, IEthSendTransaction
    {
        public EthSendTransaction(IClient client) : base(client, ApiMethods.eth_sendTransaction.ToString())
        {
        }

        public Task<string> SendRequestAsync(TransactionInput input, object id = null)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            return base.SendRequestAsync(id, input);
        }

        public RpcRequest BuildRequest(TransactionInput input, object id = null)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            return base.BuildRequest(id, input);
        }
    }
}