using System.Numerics;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexConvertors.Extensions;

namespace AlephVault.Unity.EVMGames.Nethereum.Hex.HexConvertors
{
    public class HexBigIntegerBigEndianConvertor : IHexConvertor<BigInteger>
    {
        public string ConvertToHex(BigInteger newValue)
        {
            return newValue.ToHex(false);
        }

        public BigInteger ConvertFromHex(string hex)
        {
            return hex.HexToBigInteger(false);
        }
    }
}