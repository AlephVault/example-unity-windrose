using System;
using System.Threading.Tasks;
 
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Filters
{
    /// <Summary>
    ///     eth_uninstallFilter
    ///     Uninstalls a filter with given id. Should always be called when watch is no longer needed. Additonally Filters
    ///     timeout when they aren't requested with eth_getFilterChanges for a period of time.
    ///     Parameters
    ///     QUANTITY - The filter id.
    ///     params: [
    ///     "0xb" // 11
    ///     ]
    ///     Returns
    ///     Boolean - true if the filter was successfully uninstalled, otherwise false.
    ///     Example
    ///     Request
    ///     curl -X POST --data '{"jsonrpc":"2.0","method":"eth_uninstallFilter","params":["0xb"],"id":73}'
    ///     Result
    ///     {
    ///     "id":1,
    ///     "jsonrpc": "2.0",
    ///     "result": true
    ///     }
    /// </Summary>
    public class EthUninstallFilter : RpcRequestResponseHandler<bool>, IEthUninstallFilter
    {
        public EthUninstallFilter(IClient client) : base(client, ApiMethods.eth_uninstallFilter.ToString())
        {
        }

        public Task<bool> SendRequestAsync(HexBigInteger filterId, object id = null)
        {
            if (filterId == null) throw new ArgumentNullException(nameof(filterId));
            return base.SendRequestAsync(id, filterId);
        }

        public RpcRequest BuildRequest(HexBigInteger filterId, object id = null)
        {
            if (filterId == null) throw new ArgumentNullException(nameof(filterId));
            return base.BuildRequest(id, filterId);
        }
    }
}