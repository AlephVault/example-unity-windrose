using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Admin
{
    /// <Summary>
    ///     The stopWS administrative method closes the currently open WebSocket RPC endpoint. As the node can only have a
    ///     single WebSocket endpoint running, this method takes no parameters, returning a boolean whether the endpoint was
    ///     closed or not.
    /// </Summary>
    public class AdminStopWS : GenericRpcRequestResponseHandlerNoParam<bool>, IAdminStopWS
    {
        public AdminStopWS(IClient client) : base(client, ApiMethods.admin_stopWS.ToString())
        {
        }
    }
}