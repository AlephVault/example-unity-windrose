using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using Newtonsoft.Json.Linq;


namespace AlephVault.Unity.EVMGames.Nethereum.Besu.RPC.Debug
{
    public interface IDebugStorageRangeAt
    {
        Task<JObject> SendRequestAsync(string blockHash, int txIndex, string contractAddress, string startKeyHash,
            int limitStorageEntries, object id = null);

        RpcRequest BuildRequest(string blockHash, int txIndex, string contractAddress, string startKeyHash,
            int limitStorageEntries, object id = null);
    }
}