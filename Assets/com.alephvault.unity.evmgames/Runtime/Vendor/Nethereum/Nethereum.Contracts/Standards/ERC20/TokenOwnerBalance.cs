using System.Numerics;

namespace AlephVault.Unity.EVMGames.Nethereum.Contracts.Standards.ERC20
{
    public class TokenOwnerBalance
    {
        public string ContractAddress { get; set; }
        public BigInteger Balance { get; set; }
        public string Owner { get; set; }
    }
}