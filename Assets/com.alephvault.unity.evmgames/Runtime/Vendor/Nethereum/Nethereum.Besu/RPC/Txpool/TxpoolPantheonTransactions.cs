using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure;
using Newtonsoft.Json.Linq;

namespace AlephVault.Unity.EVMGames.Nethereum.Besu.RPC.Txpool
{
    /// <Summary>
    ///     Lists transactions in the node transaction pool.
    /// </Summary>
    public class TxpoolBesuTransactions : GenericRpcRequestResponseHandlerNoParam<JArray>,
        ITxpoolBesuTransactions
    {
        public TxpoolBesuTransactions(IClient client) : base(client,
            ApiMethods.txpool_besuTransactions.ToString())
        {
        }
    }
}