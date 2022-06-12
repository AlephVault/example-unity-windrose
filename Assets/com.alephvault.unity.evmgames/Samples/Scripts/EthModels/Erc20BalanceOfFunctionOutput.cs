using AlephVault.Unity.EVMGames.Nethereum.ABI.FunctionEncoding.Attributes;
using Org.BouncyCastle.Math;


namespace AlephVault.Unity.EVMGames
{
    namespace Samples
    {
        namespace EthModels
        {
            [FunctionOutput]
            public class BalanceOfOutputDTO : IFunctionOutputDTO
            {
                [Parameter("uint256", "balance", 1)]
                public BigInteger Balance { get; set; }
            }
        }
    }
}