using AlephVault.Unity.EVMGames.Nethereum.ABI.FunctionEncoding.Attributes;
using AlephVault.Unity.EVMGames.Nethereum.Contracts;

namespace AlephVault.Unity.EVMGames.Nethereum.GSN.DTOs
{
    [Function("getHubAddr", "address")]
    public class GetHubAddrFunction : FunctionMessage
    {
    }
}
