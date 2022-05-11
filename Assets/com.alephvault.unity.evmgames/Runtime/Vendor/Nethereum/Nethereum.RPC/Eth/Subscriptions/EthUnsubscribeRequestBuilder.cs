using System;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexConvertors.Extensions;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client.Streaming;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Subscriptions
{
    public class EthUnsubscribeRequestBuilder : RpcRequestBuilder, IUnsubscribeSubscriptionRpcRequestBuilder
    {
        public EthUnsubscribeRequestBuilder() : base(ApiMethods.eth_unsubscribe.ToString())
        {
        }

        public RpcRequest BuildRequest(string subscriptionHash, object id = null)
        {
            if (id == null) id = Guid.NewGuid().ToString();
            return base.BuildRequest(id, subscriptionHash.EnsureHexPrefix());
        }
    }
}