using AlephVault.Unity.EVMGames.Nethereum.ABI.FunctionEncoding.Attributes;
using AlephVault.Unity.EVMGames.Nethereum.Contracts;

namespace AlephVault.Unity.EVMGames.Nethereum.GSN.DTOs
{
    [Function("balanceOf", "uint256")]
    public class BalanceOfFunction : FunctionMessage
    {
        [Parameter("address", "target", 1)]
        public string Target { get; set; }
    }
}
