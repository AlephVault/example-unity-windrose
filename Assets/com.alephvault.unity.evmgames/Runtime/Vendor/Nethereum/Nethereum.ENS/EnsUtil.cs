using AlephVault.Unity.EVMGames.Nethereum.ABI.Util;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexConvertors.Extensions;
using AlephVault.Unity.EVMGames.Nethereum.Util;
using System;
using System.Globalization;

namespace AlephVault.Unity.EVMGames.Nethereum.ENS
{
    public class EnsUtil
    {
        public string GetLabelHash(string label)
        {
            var kecckak = new Sha3Keccack();
            return kecckak.CalculateHash(label);
        }

        public string GetNameHash(string name)
        {
            
            var node = "0x0000000000000000000000000000000000000000000000000000000000000000";
            var kecckak = new Sha3Keccack();
            if (!string.IsNullOrEmpty(name))
            {
                name = Normalise(name);
                var labels = name.Split('.');
                for (var i = labels.Length - 1; i >= 0; i--)
                {
                    var byteInput = (node + GetLabelHash(labels[i])).HexToByteArray();
                    node = kecckak.CalculateHash(byteInput).ToHex();
                }
            }
            return node.EnsureHexPrefix();
        }

        public string Normalise(string name)
        {
            try
            {
                var idn = new IdnMapping
                {
                    UseStd3AsciiRules = true
                };
                return idn.GetAscii(name).ToLower();
                
            }
            catch (Exception ex)
            {
                throw new ArgumentOutOfRangeException("Invalid ENS name", ex);
            }
        }
    }
}