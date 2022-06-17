using System.Numerics;
using AlephVault.Unity.EVMGames.Nethereum.ABI.FunctionEncoding.Attributes;
using AlephVault.Unity.EVMGames.Nethereum.Signer.EIP712;

namespace AlephVault.Unity.EVMGames.Nethereum.GnosisSafe
{
    [Struct("EIP712Domain")]
    public class GnosisSafeEIP712Domain : IDomain
    {
        [Parameter("uint256", "chainId", 1)]
        public virtual BigInteger? ChainId { get; set; }

        [Parameter("address", "verifyingContract", 2)]
        public virtual string VerifyingContract { get; set; }

    }
}