using System.Threading.Tasks;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure
{
    public interface IGenericRpcRequestResponseHandlerNoParam<TResponse>
    {
        Task<TResponse> SendRequestAsync(object id = null);
    }
}