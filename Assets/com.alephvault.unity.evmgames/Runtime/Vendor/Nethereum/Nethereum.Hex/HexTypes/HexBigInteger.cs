using System;
using System.Numerics;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexConvertors;
using Newtonsoft.Json;

namespace AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes
{
    [JsonConverter(typeof(HexRPCTypeJsonConverter<HexBigInteger, BigInteger>))]
    public class HexBigInteger : HexRPCType<BigInteger>
    {
        public HexBigInteger(string hex) : base(new HexBigIntegerBigEndianConvertor(), hex)
        {
        }

        public HexBigInteger(BigInteger value) : base(value, new HexBigIntegerBigEndianConvertor())
        {
        }

        
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}