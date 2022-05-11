using System.Linq;
using AlephVault.Unity.EVMGames.Nethereum.ABI.Decoders;

namespace AlephVault.Unity.EVMGames.Nethereum.ABI
{
    public class EncoderDecoderHelpers
    {
        public static int GetNumberOfBytes(byte[] encoded)
        {
            var intDecoder = new IntTypeDecoder();
            var numberOfBytesEncoded = encoded.Take(32);
            return intDecoder.DecodeInt(numberOfBytesEncoded.ToArray());
        }
    }
}