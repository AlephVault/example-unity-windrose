using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.Besu.RPC.IBFT
{
    public interface IIbftGetValidatorsByBlockNumber
    {
        Task<string[]> SendRequestAsync(BlockParameter block, object id = null);
        RpcRequest BuildRequest(BlockParameter block, object id = null);
    }
}