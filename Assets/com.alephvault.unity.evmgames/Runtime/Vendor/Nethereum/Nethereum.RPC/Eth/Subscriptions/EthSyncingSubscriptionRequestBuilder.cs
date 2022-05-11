using AlephVault.Unity.EVMGames.Nethereum.Hex.HexConvertors.Extensions;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using System;
using System.Collections.Generic;
using System.Text;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.WebSocketStreamingClient;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Subscriptions
{
    public class EthSyncingSubscriptionRequestBuilder : RpcRequestBuilder
    {
        public EthSyncingSubscriptionRequestBuilder() : base(ApiMethods.eth_subscribe.ToString())
        {
        }

        public override RpcRequest BuildRequest(object id = null)
        {
            if (id == null) id = Guid.NewGuid().ToString();
            return base.BuildRequest(id, "syncing");
        }
    }
}
