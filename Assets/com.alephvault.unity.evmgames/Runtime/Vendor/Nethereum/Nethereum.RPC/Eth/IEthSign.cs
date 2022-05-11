using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Eth
{
    public interface IEthSign
    {
        RpcRequest BuildRequest(string address, string data, object id = null);
        Task<string> SendRequestAsync(string address, string data, object id = null);
    }
}