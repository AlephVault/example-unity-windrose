using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using Newtonsoft.Json.Linq;

namespace AlephVault.Unity.EVMGames.Nethereum.Parity.RPC.Trace
{
    public interface ITraceGet
    {
        RpcRequest BuildRequest(string transactionHash, HexBigInteger[] index, object id = null);
        Task<JObject> SendRequestAsync(string transactionHash, HexBigInteger[] index, object id = null);
    }
}