using System;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.Parity.RPC.PubSub
{
    public class ParitySubscribeRequestBuilder : RpcRequestBuilder
    {
        public ParitySubscribeRequestBuilder() : base(ApiMethods.parity_subscribe.ToString())
        {
        }

        public RpcRequest BuildRequest(RpcRequest originalRequestToSubscribe, object id = null)
        {
            if (id == null) id = Guid.NewGuid().ToString();
            return base.BuildRequest(id, originalRequestToSubscribe.Method, originalRequestToSubscribe.RawParameters);
        }
    }
}