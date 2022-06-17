using System.Numerics;
using AlephVault.Unity.EVMGames.Nethereum.ABI.Util;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json;

namespace AlephVault.Unity.EVMGames.Nethereum.Rsk.RPC.RskEth.DTOs
{
    public class RskBlockWithTransactionHashes : BlockWithTransactionHashes, IRskBlockExtended
    {
        /// <summary>
        ///     QUANTITY - the minimum gas price in Wei
        /// </summary>
        [JsonProperty(PropertyName = "minimumGasPrice")]
        public string MinimumGasPriceString { get; set; }

        
        

    }

}