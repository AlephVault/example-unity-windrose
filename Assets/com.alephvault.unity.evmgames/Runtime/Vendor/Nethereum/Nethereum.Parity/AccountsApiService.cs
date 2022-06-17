using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.Parity.RPC.Accounts;
using AlephVault.Unity.EVMGames.Nethereum.RPC;

namespace AlephVault.Unity.EVMGames.Nethereum.Parity
{
    public class AccountsApiService : RpcClientWrapper, IAccountsApiService
    {
        public AccountsApiService(IClient client) : base(client)
        {
            AccountsInfo = new ParityAccountsInfo(client);
            DefaultAccount = new ParityDefaultAccount(client);
            GenerateSecretPhrase = new ParityGenerateSecretPhrase(client);
            HardwareAccountsInfo = new ParityHardwareAccountsInfo(client);
        }

        public IParityAccountsInfo AccountsInfo { get; }
        public IParityDefaultAccount DefaultAccount { get; }
        public IParityGenerateSecretPhrase GenerateSecretPhrase { get; }
        public IParityHardwareAccountsInfo HardwareAccountsInfo { get; }
    }
}