using System;

namespace AlephVault.Unity.EVMGames.Nethereum.Quorum.Enclave
{
    public class QuorumEnclaveRequestException : Exception
    {
        public QuorumEnclaveRequestException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}