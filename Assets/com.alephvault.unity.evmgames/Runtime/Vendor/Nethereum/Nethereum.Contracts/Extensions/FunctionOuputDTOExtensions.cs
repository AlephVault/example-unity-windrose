using AlephVault.Unity.EVMGames.Nethereum.ABI.FunctionEncoding;
using AlephVault.Unity.EVMGames.Nethereum.ABI.FunctionEncoding.Attributes;

namespace AlephVault.Unity.EVMGames.Nethereum.Contracts
{
    public static class FunctionOuputDTOExtensions
    {
        private static readonly FunctionCallDecoder _functionCallDecoder = new FunctionCallDecoder();

        public static TFunctionOutputDTO DecodeOutput<TFunctionOutputDTO>(this TFunctionOutputDTO functionOuputDTO, string output) where TFunctionOutputDTO: IFunctionOutputDTO {
            return _functionCallDecoder.DecodeFunctionOutput(functionOuputDTO, output);
        }
    }
}