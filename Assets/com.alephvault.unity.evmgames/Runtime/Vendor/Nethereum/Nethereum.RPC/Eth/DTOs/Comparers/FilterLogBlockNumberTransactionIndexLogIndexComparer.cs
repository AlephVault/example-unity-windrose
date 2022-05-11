using System.Collections.Generic;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs.Comparers
{
    public class FilterLogBlockNumberTransactionIndexLogIndexComparer : IComparer<FilterLog>
    {
        public int Compare(FilterLog x, FilterLog y)
        {
            if (x.BlockNumber.Value != y.BlockNumber.Value)
                return x.BlockNumber.Value.CompareTo(y.BlockNumber.Value);

            if (x.TransactionIndex.Value != y.TransactionIndex.Value)
                return x.TransactionIndex.Value.CompareTo(y.TransactionIndex.Value);

            return x.LogIndex.Value.CompareTo(y.LogIndex.Value);
        }
    }
}
