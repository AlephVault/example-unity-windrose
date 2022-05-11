using System;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Subscriptions
{
    public class EthLogsSubscriptionRequestBuilder:RpcRequestBuilder
    {
        public EthLogsSubscriptionRequestBuilder() : base(ApiMethods.eth_subscribe.ToString())
        {
        }

        public RpcRequest BuildRequest(NewFilterInput filterInput, object id)
        {
            if (id == null) id = Guid.NewGuid().ToString();
            return base.BuildRequest(id, "logs", filterInput);
        }
    }
}