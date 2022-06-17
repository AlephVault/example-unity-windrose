using AlephVault.Unity.EVMGames.Nethereum.ABI.FunctionEncoding.Attributes;
using AlephVault.Unity.EVMGames.Nethereum.Contracts;

namespace AlephVault.Unity.EVMGames.Nethereum.GSN.DTOs
{
    [Function("getNonce", "uint256")]
    public class GetNonceFunction : FunctionMessage
    {
        [Parameter("address", "from", 1)]
        public string From { get; set; }
    }
}
