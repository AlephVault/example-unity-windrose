using System.Collections.Generic;
using System;

namespace AlephVault.Unity.EVMGames.Nethereum.Util
{
    public static class TransactionUtils
    {
        public static string CalculateTransactionHash(string rawSignedTransaction)
        {
            var sha3 = new Sha3Keccack();
            return sha3.CalculateHashFromHex(rawSignedTransaction);
        }
    }

    public class UniqueTransactionHashList : HashSet<string>
    {
        public UniqueTransactionHashList() : base(StringComparer.OrdinalIgnoreCase)
        {
        }
    }
}