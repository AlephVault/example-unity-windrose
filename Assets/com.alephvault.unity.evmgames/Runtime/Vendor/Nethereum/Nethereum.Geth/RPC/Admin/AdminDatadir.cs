using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure;

namespace AlephVault.Unity.EVMGames.Nethereum.Geth.RPC.Admin
{
    /// <Summary>
    ///     The datadir administrative property can be queried for the absolute path the running Geth node currently uses to
    ///     store all its databases.
    /// </Summary>
    public class AdminDatadir : GenericRpcRequestResponseHandlerNoParam<string>, IAdminDatadir
    {
        public AdminDatadir(IClient client) : base(client, ApiMethods.admin_datadir.ToString())
        {
        }
    }
}