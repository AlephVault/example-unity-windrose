using AlephVault.Unity.EVMGames.Nethereum.GSN.Models;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;
using System;
using System.Threading.Tasks;

namespace AlephVault.Unity.EVMGames.Nethereum.GSN.Interfaces
{
    public interface IRelayer
    {
        Task<string> Relay(TransactionInput transaction, Func<Relay, TransactionInput, string, Task<string>> relayFunc);
    }
}