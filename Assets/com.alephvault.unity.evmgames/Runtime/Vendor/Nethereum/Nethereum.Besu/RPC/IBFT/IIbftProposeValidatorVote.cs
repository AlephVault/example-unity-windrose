using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.Besu.RPC.IBFT
{
    public interface IIbftProposeValidatorVote
    {
        Task<bool> SendRequestAsync(string accountAddress, bool addValidator, object id = null);
        RpcRequest BuildRequest(string accountAddress, bool addValidator, object id = null);
    }
}