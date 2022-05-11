using AlephVault.Unity.EVMGames.Nethereum.ABI.Decoders;
using AlephVault.Unity.EVMGames.Nethereum.ABI.Encoders;

namespace AlephVault.Unity.EVMGames.Nethereum.ABI
{
    public class StringType : ABIType
    {
        public StringType() : base("string")
        {
            Decoder = new StringTypeDecoder();
            Encoder = new StringTypeEncoder();
        }

        public override int FixedSize => -1;
    }
}