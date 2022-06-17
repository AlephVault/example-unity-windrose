using System.Numerics;
using AlephVault.Unity.EVMGames.Nethereum.ABI.FunctionEncoding.Attributes;


namespace AlephVault.Unity.EVMGames
{
    namespace Samples
    {
        namespace EthModels
        {
            [Event("Transfer")]
            public class Erc20TransferEvent : IEventDTO
            {
                [Parameter("address", "from", 1, true)]
                public string From { get; set; }
        
                [Parameter("address", "to", 2, true)]
                public string To { get; set; }
        
                [Parameter("uint256", "value", 3)]
                public BigInteger Value { get; set; }
            }
        }
    }
}