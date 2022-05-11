using AlephVault.Unity.EVMGames.Nethereum.ABI.Decoders;
using AlephVault.Unity.EVMGames.Nethereum.ABI.Encoders;

namespace AlephVault.Unity.EVMGames.Nethereum.ABI
{
    public class BoolType : ABIType
    {
        public BoolType() : base("bool")
        {
            Decoder = new BoolTypeDecoder();
            Encoder = new BoolTypeEncoder();
        }
    }
}