using AlephVault.Unity.EVMGames.Nethereum.ABI.FunctionEncoding.Attributes;
using AlephVault.Unity.EVMGames.Nethereum.Contracts;
using Org.BouncyCastle.Math;


namespace AlephVault.Unity.EVMGames
{
    namespace Samples
    {
        namespace EthModels
        {
            [Function("transfer", "bool")]
            public class Erc20TransferFunction : FunctionMessage
            {
                [Parameter("address", "_to", 1)]
                public string To { get; set; }

                [Parameter("uint256", "_value", 2)]
                public BigInteger TokenAmount { get; set; }
            }
        }
    }
}