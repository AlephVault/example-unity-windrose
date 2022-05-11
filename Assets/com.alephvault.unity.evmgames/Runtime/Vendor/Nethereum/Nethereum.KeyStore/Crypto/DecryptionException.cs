using System;

namespace AlephVault.Unity.EVMGames.Nethereum.KeyStore.Crypto
{
    public class DecryptionException : Exception
    {
        internal DecryptionException(string msg) : base(msg)
        {
        }
    }
}