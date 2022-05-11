using System;
using System.Collections.Generic;
using AlephVault.Unity.EVMGames.Nethereum.Contracts;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs.Comparers
{
    public class EventLogBlockNumberTransactionIndexComparer : IComparer<object>
    {
        public int Compare(object x, object y)
        {
            var xLog = x as IEventLog;
            var yLog = y as IEventLog;
            if (xLog == null || yLog == null) throw new Exception("Both instances should implement IEventLog");
            return new FilterLogBlockNumberTransactionIndexLogIndexComparer().Compare(xLog.Log, yLog.Log);
        }
    }


    public class EventLogBlockNumberTransactionIndexComparer<TEventLog> : IComparer<TEventLog> where TEventLog : IEventLog
    {
        public int Compare(TEventLog x, TEventLog y)
        {
            var xLog = x as IEventLog;
            var yLog = y as IEventLog;
            if (xLog == null || yLog == null) throw new Exception("Both instances should implement IEventLog");
            return new FilterLogBlockNumberTransactionIndexLogIndexComparer().Compare(xLog.Log, yLog.Log);
        }
    }
}