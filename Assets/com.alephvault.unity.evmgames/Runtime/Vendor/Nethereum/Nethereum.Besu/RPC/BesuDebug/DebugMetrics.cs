using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure;
using Newtonsoft.Json.Linq;

namespace AlephVault.Unity.EVMGames.Nethereum.Besu.RPC.Debug
{
    /// <Summary>
    ///     Returns metrics providing information on the internal operation of Besu.
    ///     The available metrics may change over time. The JVM metrics may vary based on the JVM implementation being used.
    ///     The metric types are:
    ///     Timer
    ///     Counter
    ///     Gauge.
    /// </Summary>
    public class DebugMetrics : GenericRpcRequestResponseHandlerNoParam<JObject>, IDebugMetrics
    {
        public DebugMetrics(IClient client) : base(client, ApiMethods.debug_metrics.ToString())
        {
        }
    }
}