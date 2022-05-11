using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Personal
{
    public interface IPersonalNewAccount
    {
        RpcRequest BuildRequest(string passPhrase, object id = null);
        Task<string> SendRequestAsync(string passPhrase, object id = null);
    }
}