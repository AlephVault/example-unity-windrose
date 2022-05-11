using AlephVault.Unity.EVMGames.Nethereum.ABI.Decoders;
using AlephVault.Unity.EVMGames.Nethereum.ABI.Encoders;

namespace AlephVault.Unity.EVMGames.Nethereum.ABI
{
    public class DynamicArrayType : ArrayType
    {
        public DynamicArrayType(string name) : base(name)
        {
            Decoder = new DynamicArrayTypeDecoder(ElementType);
            Encoder = new DynamicArrayTypeEncoder(ElementType);
        }

        public override string CanonicalName => ElementType.CanonicalName + "[]";

        public override int FixedSize => -1;
    }
}