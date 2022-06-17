using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.Besu.RPC.IBFT
{
    public interface IIbftGetValidatorsByBlockHash
    {
        Task<string[]> SendRequestAsync(string blockHash, object id = null);
        RpcRequest BuildRequest(string blockHash, object id = null);
    }
}