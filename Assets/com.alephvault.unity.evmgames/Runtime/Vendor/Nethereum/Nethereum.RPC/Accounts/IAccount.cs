using AlephVault.Unity.EVMGames.Nethereum.RPC.TransactionManagers;
using System;
using System.Collections.Generic;
using System.Text;
using AlephVault.Unity.EVMGames.Nethereum.RPC.NonceServices;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Accounts
{
    public interface IAccount
    {
        string Address { get; }
        ITransactionManager TransactionManager { get; }

        INonceService NonceService { get; set; }
    }
}
