using System;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexConvertors.Extensions;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client.Streaming;

namespace AlephVault.Unity.EVMGames.Nethereum.Parity.RPC.PubSub
{
    public class ParityUnsubscribeRequestBuilder : RpcRequestBuilder, IUnsubscribeSubscriptionRpcRequestBuilder
    {
        public ParityUnsubscribeRequestBuilder() : base(ApiMethods.parity_unsubscribe.ToString())
        {
        }

        public RpcRequest BuildRequest(string subscriptionHash, object id = null)
        {
            if (id == null) id = Guid.NewGuid().ToString();
            return base.BuildRequest(id, subscriptionHash.EnsureHexPrefix());
        }
    }
}