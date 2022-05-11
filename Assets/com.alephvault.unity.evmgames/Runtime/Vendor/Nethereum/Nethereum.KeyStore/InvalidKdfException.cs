using System;

namespace AlephVault.Unity.EVMGames.Nethereum.KeyStore
{
    public class InvalidKdfException : Exception
    {
        public InvalidKdfException(string kdf) : base("Invalid kdf:" + kdf)
        {
        }
    }
}