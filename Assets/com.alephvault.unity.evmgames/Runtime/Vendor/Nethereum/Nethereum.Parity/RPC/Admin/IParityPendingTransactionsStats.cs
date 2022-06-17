using AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure;
using Newtonsoft.Json.Linq;

namespace AlephVault.Unity.EVMGames.Nethereum.Parity.RPC.Admin
{
    public interface IParityPendingTransactionsStats : IGenericRpcRequestResponseHandlerNoParam<JObject>
    {

    }
}