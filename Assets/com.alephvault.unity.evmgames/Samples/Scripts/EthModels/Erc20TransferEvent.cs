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
                [Parameter("address", "_from", 1)]
                public string From { get; set; }
        
                [Parameter("address", "_to", 2)]        
                public string To { get; set; }
        
                [Parameter("uint256", "_value", 3)]        
                public BigInteger Value { get; set; }
            }
        }
    }
}