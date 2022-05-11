using AlephVault.Unity.EVMGames.WalletConnectSharp.Core.Models;

namespace AlephVault.Unity.EVMGames.WalletConnectSharp.Core.Events.Request
{
    public class JsonRpcResponseEvent<T> : GenericEvent<T> where T : JsonRpcResponse
    {
    }
}