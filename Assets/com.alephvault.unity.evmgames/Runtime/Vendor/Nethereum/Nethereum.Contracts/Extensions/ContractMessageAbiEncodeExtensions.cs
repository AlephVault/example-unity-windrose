using AlephVault.Unity.EVMGames.Nethereum.ABI;
using AlephVault.Unity.EVMGames.Nethereum.Contracts.CQS;

namespace AlephVault.Unity.EVMGames.Nethereum.Contracts
{
    public static class ContractMessageAbiEncodeExtensions
    {
        public static byte[] GetParamsEncoded<TContractMessage>(this TContractMessage contractMessage) where TContractMessage : ContractMessageBase
        {
            var encoder = new ABIEncode();
            return encoder.GetABIParamsEncoded(contractMessage);
        }

        public static byte[] GetParamsEncodedPacked<TContractMessage>(this TContractMessage contractMessage) where TContractMessage : ContractMessageBase
        {
            var encoder = new ABIEncode();
            return encoder.GetABIParamsEncodedPacked(contractMessage);
        }

        public static byte[] GetSha3ParamsEncoded<TContractMessage>(this TContractMessage contractMessage) where TContractMessage : ContractMessageBase
        {
            var encoder = new ABIEncode();
            return encoder.GetSha3ABIParamsEncoded(contractMessage);
        }

        public static byte[] GetSha3ParamsEncodedPacked<TContractMessage>(this TContractMessage contractMessage) where TContractMessage : ContractMessageBase
        {
            var encoder = new ABIEncode();
            return encoder.GetSha3ABIParamsEncodedPacked(contractMessage);
        }

    }
}