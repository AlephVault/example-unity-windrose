using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using Newtonsoft.Json.Linq;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Debug
{
    public interface IDebugDumpBlock
    {
        RpcRequest BuildRequest(ulong blockNumber, object id = null);
        Task<JObject> SendRequestAsync(ulong blockNumber, object id = null);
    }
}