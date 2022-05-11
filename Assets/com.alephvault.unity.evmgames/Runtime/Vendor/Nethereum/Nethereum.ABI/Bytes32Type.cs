using AlephVault.Unity.EVMGames.Nethereum.ABI.Decoders;
using AlephVault.Unity.EVMGames.Nethereum.ABI.Encoders;

namespace AlephVault.Unity.EVMGames.Nethereum.ABI
{
    public class Bytes32Type : ABIType
    {
        public Bytes32Type(string name) : base(name)
        {
            Decoder = new Bytes32TypeDecoder();
            Encoder = new Bytes32TypeEncoder();
        }
    }
}