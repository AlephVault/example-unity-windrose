using AlephVault.Unity.EVMGames.Nethereum.Contracts.CQS;
using AlephVault.Unity.EVMGames.Nethereum.Util;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexConvertors.Extensions;
namespace AlephVault.Unity.EVMGames.Nethereum.Contracts
{
    public class ContractDeploymentMessage : ContractMessageBase
    {

        public ContractDeploymentMessage(string byteCode)
        {
            ByteCode = byteCode;
        }

        /// <summary>
        /// ByteCode (Compiled code) used for deployment
        /// </summary>
        public string ByteCode { get; internal set; }

    }
}