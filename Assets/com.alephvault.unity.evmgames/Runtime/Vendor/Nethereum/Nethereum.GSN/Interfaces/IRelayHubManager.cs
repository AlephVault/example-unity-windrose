﻿using System.Numerics;
using System.Threading.Tasks;

namespace AlephVault.Unity.EVMGames.Nethereum.GSN.Interfaces
{
    public interface IRelayHubManager
    {
        Task<string> GetHubAddressAsync(string contractAddress);
        Task<BigInteger> GetNonceAsync(string hubAddress, string from);
        Task<RelayCollection> GetRelaysAsync(string hubAddress, IRelayPriorityPolicy policy);
    }
}