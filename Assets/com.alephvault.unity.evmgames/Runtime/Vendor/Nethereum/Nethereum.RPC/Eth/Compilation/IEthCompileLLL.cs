using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using Newtonsoft.Json.Linq;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Compilation
{
    public interface IEthCompileLLL
    {
        RpcRequest BuildRequest(string lllcode, object id = null);
        Task<JObject> SendRequestAsync(string lllcode, object id = null);
    }
}