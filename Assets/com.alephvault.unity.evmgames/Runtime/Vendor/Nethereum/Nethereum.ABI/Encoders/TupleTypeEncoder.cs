using System;
using AlephVault.Unity.EVMGames.Nethereum.ABI.Encoders;
using AlephVault.Unity.EVMGames.Nethereum.ABI.FunctionEncoding;
using AlephVault.Unity.EVMGames.Nethereum.ABI.Model;

namespace AlephVault.Unity.EVMGames.Nethereum.ABI
{
    public class TupleTypeEncoder : ITypeEncoder
    {
        private readonly ParametersEncoder parametersEncoder;

        public TupleTypeEncoder()
        {
            parametersEncoder = new ParametersEncoder();
        }

        public Parameter[] Components { get; set; }

        public byte[] Encode(object value)
        {
            if (!(value == null || value is object[]))
                return parametersEncoder.EncodeParametersFromTypeAttributes(value.GetType(), value);

            var input = value as object[];
            return parametersEncoder.EncodeParameters(Components, input);
        }

        public byte[] EncodePacked(object value)
        {
            throw new NotImplementedException();
        }
    }
}