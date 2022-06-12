using AlephVault.Unity.EVMGames.Nethereum.ABI.FunctionEncoding.Attributes;
using AlephVault.Unity.EVMGames.Nethereum.Contracts;


namespace AlephVault.Unity.EVMGames
{
    namespace Samples
    {
        namespace EthModels
        {
            [Function("balanceOf", "uint256")]
            public class Erc20BalanceOfFunction : FunctionMessage
            {
                [Parameter("address", "_owner", 1)]
                public string Owner { get; set; }
            }
        }
    }
}