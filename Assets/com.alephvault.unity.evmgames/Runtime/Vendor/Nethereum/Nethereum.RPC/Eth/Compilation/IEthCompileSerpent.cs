using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using Newtonsoft.Json.Linq;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Compilation
{
    public interface IEthCompileSerpent
    {
        RpcRequest BuildRequest(string serpentCode, object id = null);
        Task<JObject> SendRequestAsync(string serpentCode, object id = null);
    }
}