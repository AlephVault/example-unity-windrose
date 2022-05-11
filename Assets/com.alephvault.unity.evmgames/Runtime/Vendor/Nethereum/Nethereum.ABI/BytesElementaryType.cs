using AlephVault.Unity.EVMGames.Nethereum.ABI.Decoders;
using AlephVault.Unity.EVMGames.Nethereum.ABI.Encoders;

namespace AlephVault.Unity.EVMGames.Nethereum.ABI
{
    public class BytesElementaryType : ABIType
    {
        public BytesElementaryType(string name, int size) : base(name)
        {
            Decoder = new BytesElementaryTypeDecoder(size);
            Encoder = new BytesElementaryTypeEncoder(size);
        }
    }
}