using System;
using System.Collections.Generic;
using System.Text;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexConvertors.Extensions;

namespace AlephVault.Unity.EVMGames.Nethereum.Signer
{
    public class EthereumMessageSigner : MessageSigner
    {
        public override string EcRecover(byte[] message, string signature)
        {
            return base.EcRecover(HashPrefixedMessage(message), signature);
        }
        
        public override string EcRecover(byte[] message, EthECDSASignature signature)
        {
            return base.EcRecover(HashPrefixedMessage(message), signature);
        }

        public virtual Tuple<string, byte[]> EcFullRecover(byte[] message, string signature)
        {
            return base.EcFullRecover(HashPrefixedMessage(message), signature);
        }

        public virtual Tuple<string, byte[]> EcFullRecover(byte[] message, EthECDSASignature signature)
        {
            return base.EcFullRecover(HashPrefixedMessage(message), signature);
        }

        public byte[] HashAndHashPrefixedMessage(byte[] message)
        {
            return HashPrefixedMessage(Hash(message));
        }

        public override string HashAndSign(byte[] plainMessage, EthECKey key)
        {
            return base.Sign(HashAndHashPrefixedMessage(plainMessage), key);
        }

        public byte[] HashPrefixedMessage(byte[] message)
        {
            var byteList = new List<byte>();
            var bytePrefix = "0x19".HexToByteArray();
            var textBytePrefix = Encoding.UTF8.GetBytes("Ethereum Signed Message:\n" + message.Length);

            byteList.AddRange(bytePrefix);
            byteList.AddRange(textBytePrefix);
            byteList.AddRange(message);
            return Hash(byteList.ToArray());
        }

        public override string Sign(byte[] message, EthECKey key)
        {
            return base.Sign(HashPrefixedMessage(message), key);
        }

        public string EncodeUTF8AndSign(string message, EthECKey key)
        {
            return base.Sign(HashPrefixedMessage(Encoding.UTF8.GetBytes(message)), key);
        }

        public string EncodeUTF8AndEcRecover(string message, string signature)
        {
            return EcRecover(Encoding.UTF8.GetBytes(message), signature);
        }
        
        public Tuple<string, byte[]> EncodeUTF8AndEcFullRecover(string message, string signature)
        {
            return EcFullRecover(Encoding.UTF8.GetBytes(message), signature);
        }
    }
}