using AlephVault.Unity.EVMGames.WalletConnectSharp.Core.Models;

namespace AlephVault.Unity.EVMGames.WalletConnectSharp.Core.Events.Response
{
    public class JsonRpcRequestEvent<T> : GenericEvent<T> where T : JsonRpcRequest
    {
    }
}