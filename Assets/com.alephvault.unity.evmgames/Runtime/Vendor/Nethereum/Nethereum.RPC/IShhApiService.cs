using AlephVault.Unity.EVMGames.Nethereum.RPC.Shh;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC
{
    public interface IShhApiService
    {
        IShhKeyPair KeyPair { get; } 
        IShhSymKey SymKey { get; } 
        IShhMessageFilter MessageFilter { get; }
        IShhPost Post { get; }
        IShhVersion Version { get; }
    }
}