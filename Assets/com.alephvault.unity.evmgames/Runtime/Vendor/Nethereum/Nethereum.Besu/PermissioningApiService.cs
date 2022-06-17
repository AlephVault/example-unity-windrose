using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.Besu.RPC.Permissioning;
using AlephVault.Unity.EVMGames.Nethereum.RPC;

namespace AlephVault.Unity.EVMGames.Nethereum.Besu
{
    public class PermissioningApiService : RpcClientWrapper, IPermissioningApiService
    {
        public PermissioningApiService(IClient client) : base(client)
        {
            AddAccountsToWhitelist = new PermAddAccountsToWhitelist(client);
            AddNodesToWhitelist = new PermAddNodesToWhitelist(client);
            RemoveAccountsFromWhitelist = new PermRemoveAccountsFromWhitelist(client);
            RemoveNodesFromWhitelist = new PermRemoveNodesFromWhitelist(client);
            GetAccountsWhitelist = new PermGetAccountsWhitelist(client);
            GetNodesWhitelist = new PermGetNodesWhitelist(client);
            ReloadPermissionsFromFile = new PermReloadPermissionsFromFile(client);
        }

        public IPermAddAccountsToWhitelist AddAccountsToWhitelist { get; }
        public IPermAddNodesToWhitelist AddNodesToWhitelist { get; }
        public IPermRemoveAccountsFromWhitelist RemoveAccountsFromWhitelist { get; }
        public IPermRemoveNodesFromWhitelist RemoveNodesFromWhitelist { get; }
        public IPermGetAccountsWhitelist GetAccountsWhitelist { get; }
        public IPermGetNodesWhitelist GetNodesWhitelist { get; }
        public IPermReloadPermissionsFromFile ReloadPermissionsFromFile { get; }
    }
}