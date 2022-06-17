using System;
using System.Collections.Generic;
using System.Text;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json;

namespace AlephVault.Unity.EVMGames.Nethereum.Rsk.RPC.RskEth.DTOs
{
    public class RskBlockWithTransactions:BlockWithTransactions, IRskBlockExtended
    {
            /// <summary>
            ///     QUANTITY - the minimum gas price in Wei
            /// </summary>
            [JsonProperty(PropertyName = "minimumGasPrice")]
            public string MinimumGasPriceString { get; set; }
    }
}
