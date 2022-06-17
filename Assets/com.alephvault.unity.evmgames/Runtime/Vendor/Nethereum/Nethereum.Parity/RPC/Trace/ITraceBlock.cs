using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using Newtonsoft.Json.Linq;

namespace AlephVault.Unity.EVMGames.Nethereum.Parity.RPC.Trace
{
    public interface ITraceBlock
    {
        RpcRequest BuildRequest(HexBigInteger blockNumber, object id = null);
        Task<JArray> SendRequestAsync(HexBigInteger blockNumber, object id = null);
    }
}