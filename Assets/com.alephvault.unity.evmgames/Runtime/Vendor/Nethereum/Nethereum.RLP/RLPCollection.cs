using System.Collections.Generic;

namespace AlephVault.Unity.EVMGames.Nethereum.RLP
{
    public class RLPCollection : List<IRLPElement>, IRLPElement
    {
        public byte[] RLPData { get; set; }
    }
}