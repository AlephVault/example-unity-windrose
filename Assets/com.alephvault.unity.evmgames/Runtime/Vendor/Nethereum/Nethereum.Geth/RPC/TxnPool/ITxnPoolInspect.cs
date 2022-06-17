using AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure;
using Newtonsoft.Json.Linq;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.TxnPool
{
    public interface ITxnPoolInspect: IGenericRpcRequestResponseHandlerNoParam<JObject>
    {
    }
}