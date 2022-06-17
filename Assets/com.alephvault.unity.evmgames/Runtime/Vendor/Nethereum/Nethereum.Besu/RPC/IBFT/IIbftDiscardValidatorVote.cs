using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.Besu.RPC.IBFT
{
    public interface IIbftDiscardValidatorVote
    {
        Task<bool> SendRequestAsync(string validatorAddress, object id = null);
        RpcRequest BuildRequest(string validatorAddress, object id = null);
    }
}