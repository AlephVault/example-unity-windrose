using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Contracts.ContractHandlers;
using AlephVault.Unity.EVMGames.Nethereum.Contracts.Services;
using AlephVault.Unity.EVMGames.Nethereum.Contracts.Standards.ERC1271.ContractDefinition;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexConvertors.Extensions;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.Contracts.Standards.ERC1271
{
    public class ERC1271ContractService
    {
        public const string MAGICVALUE = "0x1626ba7e";
        public string ContractAddress { get; }
        public ContractHandler ContractHandler { get; }

        public ERC1271ContractService(IEthApiContractService ethApiContractService, string contractAddress)
        {
            ContractAddress = contractAddress;
#if !DOTNET35
            ContractHandler = ethApiContractService.GetContractHandler(contractAddress);
#endif
        }

#if !DOTNET35
        public Task<byte[]> IsValidSignatureQueryAsync(IsValidSignatureFunction isValidSignature, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<IsValidSignatureFunction, byte[]>(isValidSignature, blockParameter);
        }

        public Task<byte[]> IsValidSignatureQueryAsync(string hash, string signature, BlockParameter blockParameter = null)
        {
            return IsValidSignatureQueryAsync(hash.HexToByteArray(), signature.HexToByteArray(), blockParameter);
        }

        public Task<byte[]> IsValidSignatureQueryAsync(byte[] hash, byte[] signature, BlockParameter blockParameter = null)
        {
            var isValidSignatureFunction = new IsValidSignatureFunction();
            isValidSignatureFunction.Hash = hash;
            isValidSignatureFunction.Signature = signature;
            return ContractHandler.QueryAsync<IsValidSignatureFunction, byte[]>(isValidSignatureFunction, blockParameter);
        }

        public async Task<bool> IsValidSignatureAndValidateReturnQueryAsync(byte[] hash, byte[] signature, BlockParameter blockParameter = null)
        {
            var returnOutput = await IsValidSignatureQueryAsync(hash, signature, blockParameter).ConfigureAwait(false);
            return IsValidSignatureOutputTheSameAsMagicValue(returnOutput);
        }

        public async Task<bool> IsValidSignatureAndValidateReturnQueryAsync(string hash, string signature, BlockParameter blockParameter = null)
        {
            var returnOutput = await IsValidSignatureQueryAsync(hash, signature, blockParameter).ConfigureAwait(false);
            return IsValidSignatureOutputTheSameAsMagicValue(returnOutput);
        }
        public static bool IsValidSignatureOutputTheSameAsMagicValue(byte[] returnOutput)
        {
            return returnOutput.ToHex().IsTheSameHex(MAGICVALUE);
        }

#endif
    }
}