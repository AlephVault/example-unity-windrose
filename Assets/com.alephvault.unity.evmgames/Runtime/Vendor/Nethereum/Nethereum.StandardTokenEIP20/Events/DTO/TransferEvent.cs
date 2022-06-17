using System;
using System.Numerics;
using AlephVault.Unity.EVMGames.Nethereum.ABI.FunctionEncoding.Attributes;
using AlephVault.Unity.EVMGames.Nethereum.Contracts;

namespace AlephVault.Unity.EVMGames.Nethereum.StandardTokenEIP20.Events.DTO
{
    [Event("Transfer")]
    [Obsolete("Please use TransferEventDTO instead")]
    public class Transfer : IEventDTO
    {
        [Parameter("address", "from", 1, true)]
        public string AddressFrom { get; set; }

        [Parameter("address", "to", 2, true)]
        public string AddressTo { get; set; }

        [Parameter("uint", "value", 3)]
        public BigInteger Value { get; set; }
    }
}