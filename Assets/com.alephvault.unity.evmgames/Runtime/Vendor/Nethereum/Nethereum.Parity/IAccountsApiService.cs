using AlephVault.Unity.EVMGames.Nethereum.Parity.RPC.Accounts;

namespace AlephVault.Unity.EVMGames.Nethereum.Parity
{
    public interface IAccountsApiService
    {
        IParityAccountsInfo AccountsInfo { get; }
        IParityDefaultAccount DefaultAccount { get; }
        IParityGenerateSecretPhrase GenerateSecretPhrase { get; }
        IParityHardwareAccountsInfo HardwareAccountsInfo { get; }
    }
}