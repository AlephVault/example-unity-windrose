using AlephVault.Unity.EVMGames.Nethereum.RPC.Personal;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC
{
    public interface IPersonalApiService
    {
        IPersonalListAccounts ListAccounts { get; }
        IPersonalLockAccount LockAccount { get; }
        IPersonalNewAccount NewAccount { get; }
        IPersonalSignAndSendTransaction SignAndSendTransaction { get; }
        IPersonalUnlockAccount UnlockAccount { get; }
    }
}