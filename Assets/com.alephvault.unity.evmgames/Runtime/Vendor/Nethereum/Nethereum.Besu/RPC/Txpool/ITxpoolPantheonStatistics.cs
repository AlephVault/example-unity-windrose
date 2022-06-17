using AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure;
using Newtonsoft.Json.Linq;

namespace AlephVault.Unity.EVMGames.Nethereum.Besu.RPC.Txpool
{
    public interface ITxpoolBesuStatistics : IGenericRpcRequestResponseHandlerNoParam<JObject>
    {
    }
}