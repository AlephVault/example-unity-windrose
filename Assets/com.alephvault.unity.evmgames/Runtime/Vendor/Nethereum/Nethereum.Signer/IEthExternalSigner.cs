using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Signer.Crypto;

namespace AlephVault.Unity.EVMGames.Nethereum.Signer
{
#if !DOTNET35

    public enum ExternalSignerTransactionFormat
    {
        RLP,
        Hash,
        Transaction
    }
    
#endif
}