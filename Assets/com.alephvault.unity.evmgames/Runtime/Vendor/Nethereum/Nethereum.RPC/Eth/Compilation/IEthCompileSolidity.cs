using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using Newtonsoft.Json.Linq;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Compilation
{
    public interface IEthCompileSolidity
    {
        RpcRequest BuildRequest(string contractCode, object id = null);
        Task<JToken> SendRequestAsync(string contractCode, object id = null);
    }
}