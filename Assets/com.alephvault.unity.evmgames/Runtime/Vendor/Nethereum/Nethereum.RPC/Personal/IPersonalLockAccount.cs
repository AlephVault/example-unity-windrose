using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Personal
{
    public interface IPersonalLockAccount
    {
        RpcRequest BuildRequest(string account, object id = null);
        Task<bool> SendRequestAsync(string account, object id = null);
    }
}