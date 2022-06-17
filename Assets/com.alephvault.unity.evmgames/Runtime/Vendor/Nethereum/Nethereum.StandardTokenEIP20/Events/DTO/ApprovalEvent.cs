using System;
using System.Numerics;
using AlephVault.Unity.EVMGames.Nethereum.ABI.FunctionEncoding.Attributes;
using AlephVault.Unity.EVMGames.Nethereum.Contracts;
using AlephVault.Unity.EVMGames.Nethereum.Contracts.Extensions;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.StandardTokenEIP20.Events.DTO
{
    [Event("Approval")]
    [Obsolete("Please use ApprovalEventDTO instead")]
    public partial class Approval : IEventDTO
    {
        [Parameter("address", "owner", 1, true)]
        public string AddressOwner { get; set; }

        [Parameter("address", "spender", 2, true)]
        public string AddressSpender { get; set; }

        [Parameter("uint", "value", 3)]
        public BigInteger Value { get; set; }
    }
}