using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure;
using Newtonsoft.Json.Linq;

namespace AlephVault.Unity.EVMGames.Nethereum.Besu.RPC.Txpool
{
    /// <Summary>
    ///     Lists statistics about the node transaction pool.
    /// </Summary>
    public class TxpoolBesuStatistics : GenericRpcRequestResponseHandlerNoParam<JObject>, ITxpoolBesuStatistics
    {
        public TxpoolBesuStatistics(IClient client) : base(client, ApiMethods.txpool_besuStatistics.ToString())
        {
        }
    }
}