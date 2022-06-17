using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using AlephVault.Unity.EVMGames.Nethereum.ABI.FunctionEncoding.Attributes;

namespace AlephVault.Unity.EVMGames.Nethereum.Optimism.L1CrossDomainMessenger.ContractDefinition
{
    public partial class ChainInclusionProof : ChainInclusionProofBase { }

    public class ChainInclusionProofBase
    {
        [Parameter("uint256", "index", 1)]
        public virtual BigInteger Index { get; set; }
        [Parameter("bytes32[]", "siblings", 2)]
        public virtual List<byte[]> Siblings { get; set; }
    }
}
